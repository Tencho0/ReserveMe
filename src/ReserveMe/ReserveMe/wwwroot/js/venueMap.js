window.venueMap = (function () {

    function parseCenter(lat, lng) {
        const parsedLat = typeof lat === "string" ? parseFloat(lat) : lat;
        const parsedLng = typeof lng === "string" ? parseFloat(lng) : lng;
        return { lat: parsedLat, lng: parsedLng };
    }

    async function init(apiKey, elementId, lat, lng, title) {
        if (!apiKey || typeof apiKey !== "string" || apiKey.trim() === "") {
            console.error("[venueMap] Missing Google Maps API key.");
            return;
        }

        const el = document.getElementById(elementId);
        if (!el) {
            console.warn("[venueMap] Map container not found:", elementId);
            return;
        }

        const center = parseCenter(lat, lng);
        if (!Number.isFinite(center.lat) || !Number.isFinite(center.lng)) {
            console.error("[venueMap] Invalid coordinates:", lat, lng);
            return;
        }

        try {
            console.log("[venueMap] Attempting modern load...");
            await window.mapLoader.loadModern(apiKey);

            if (google.maps && typeof google.maps.importLibrary === "function") {
                const { Map } = await google.maps.importLibrary("maps");

                const map = new Map(el, {
                    center,
                    zoom: 15,
                    mapTypeControl: false,
                    streetViewControl: false,
                    fullscreenControl: false
                });

                const marker = new google.maps.Marker({
                    map,
                    position: center,
                    title: title || "Venue"
                });

                console.log("[venueMap] Map created successfully (modern)");
                el.__gm_instance__ = map;
                return;
            }
        } catch (e) {
            console.warn("[venueMap] Modern load failed, trying legacy:", e);
        }

        try {
            await window.mapLoader.loadLegacy(apiKey);

            if (!(google.maps && typeof google.maps.Map === "function")) {
                console.error("[venueMap] Legacy API loaded but Map unavailable.");
                return;
            }

            const map = new google.maps.Map(el, {
                center,
                zoom: 15,
                mapTypeControl: false,
                streetViewControl: false,
                fullscreenControl: false
            });

            const marker = new google.maps.Marker({
                position: center,
                map,
                title: title || "Venue"
            });

            console.log("[venueMap] Map created successfully (legacy)");
            el.__gm_instance__ = map;

        } catch (err) {
            console.error("[venueMap] Both loads failed:", err);
            el.innerHTML = '<div style="padding: 20px; text-align: center; color: #666;">Unable to load map</div>';
        }
    }

    return { init };
})();