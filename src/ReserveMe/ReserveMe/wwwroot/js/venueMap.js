window.venueMap = (function () {
    let primaryLoadPromise;
    let legacyLoadPromise;

    function injectScript(src) {
        return new Promise((resolve, reject) => {
            const s = document.createElement("script");
            s.src = src;
            s.async = true;
            s.defer = true;
            s.onload = () => resolve();
            s.onerror = () => reject(new Error("Failed to load Google Maps JavaScript API."));
            document.head.appendChild(s);
        });
    }

    function loadMapsModern(apiKey) {
        if (window.google && window.google.maps && google.maps.importLibrary) return Promise.resolve();
        if (!primaryLoadPromise) {
            if (!apiKey) return Promise.reject(new Error("Google Maps API key is missing."));

            const src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}&v=beta&loading=async`;
            primaryLoadPromise = injectScript(src);
        }
        return primaryLoadPromise;
    }

    function loadMapsLegacy(apiKey) {
        if (window.google && window.google.maps && typeof google.maps.Map === "function") return Promise.resolve();
        if (!legacyLoadPromise) {
            if (!apiKey) return Promise.reject(new Error("Google Maps API key is missing."));

            const src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}&v=quarterly&libraries=marker`;
            legacyLoadPromise = injectScript(src);
        }
        return legacyLoadPromise;
    }

    function createGoldStar() {
        const span = document.createElement("span");
        span.textContent = "★";
        span.style.color = "#f59e0b";
        span.style.fontSize = "20px";
        span.style.lineHeight = "1";
        span.style.textShadow = "0 0 1px rgba(0,0,0,0.25)";
        return span;
    }

    function getGoldPinIconLegacy() {
        return {
            path: "M12 2C8.134 2 5 5.134 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.866-3.134-7-7-7zm0 11.5A2.5 2.5 0 1 1 12 6.5a2.5 2.5 0 0 1 0 5z",
            fillColor: "#f59e0b",
            fillOpacity: 1,
            strokeColor: "#b45309",
            strokeWeight: 1,
            scale: 1.5,
            anchor: new google.maps.Point(12, 22)
        };
    }

    async function init(apiKey, elementId, lat, lng, title) {
        try {
            await loadMapsModern(apiKey);
            if (google.maps && typeof google.maps.importLibrary === "function") {
                const el = document.getElementById(elementId);
                if (!el) {
                    console.warn("[venueMap] Map container not found:", elementId);
                    return;
                }

                const { Map } = await google.maps.importLibrary("maps");
                const { AdvancedMarkerElement } = await google.maps.importLibrary("marker");

                const center = {
                    lat: typeof lat === "string" ? parseFloat(lat) : lat,
                    lng: typeof lng === "string" ? parseFloat(lng) : lng
                };

                const map = new Map(el, {
                    center,
                    zoom: 15,
                    mapTypeControl: false,
                    streetViewControl: false,
                    fullscreenControl: false
                });

                new AdvancedMarkerElement({
                    map,
                    position: center,
                    title: title || "Venue",
                    content: createGoldStar()
                });

                el.__gm_instance__ = map;
                return;
            }
        } catch (e) {
            console.warn("[venueMap] Modern load failed, will try legacy. Reason:", e);
        }

        try {
            await loadMapsLegacy(apiKey);

            if (!(google.maps && typeof google.maps.Map === "function")) {
                console.error("[venueMap] Legacy API loaded but google.maps.Map is unavailable.");
                return;
            }

            const el = document.getElementById(elementId);
            if (!el) {
                console.warn("[venueMap] Map container not found:", elementId);
                return;
            }

            const center = {
                lat: typeof lat === "string" ? parseFloat(lat) : lat,
                lng: typeof lng === "string" ? parseFloat(lng) : lng
            };

            const map = new google.maps.Map(el, {
                center,
                zoom: 15,
                mapTypeControl: false,
                streetViewControl: false,
                fullscreenControl: false
            });

            new google.maps.Marker({
                position: center,
                map,
                title: title || "Venue",
                icon: getGoldPinIconLegacy()
            });

            el.__gm_instance__ = map;
        } catch (err) {
            console.error("[venueMap] Legacy load also failed:", err);
        }
    }

    return { init };
})();
