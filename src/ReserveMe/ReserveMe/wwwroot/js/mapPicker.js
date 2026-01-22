window.mapPicker = (function () {

    async function init(elementId, apiKey, lat, lng, dotNetHelper) {
        const el = document.getElementById(elementId);
        if (!el) {
            console.error("[mapPicker] Element not found:", elementId);
            return;
        }

        // Default to Sofia
        const center = {
            lat: (lat === 0 && lng === 0) ? 42.6977 : lat,
            lng: (lat === 0 && lng === 0) ? 23.3219 : lng
        };

        let map = null;
        let marker = null;

        try {
            console.log("[mapPicker] Attempting modern load...");
            await window.mapLoader.loadModern(apiKey);

            if (!window.google || !window.google.maps) {
                throw new Error("Maps API not ready after load");
            }

            if (google.maps.importLibrary) {
                const { Map } = await google.maps.importLibrary("maps");
                map = new Map(el, {
                    center,
                    zoom: 15,
                    streetViewControl: false,
                    mapTypeControl: false,
                    fullscreenControl: false
                });

                console.log("[mapPicker] Map created (modern)");

                marker = new google.maps.Marker({
                    position: center,
                    map: map,
                    draggable: true,
                    title: "Drag me to set location"
                });

                console.log("[mapPicker] Standard marker created");
            } else {
                throw new Error("importLibrary not available");
            }

        } catch (modernError) {
            console.warn("[mapPicker] Modern load failed, trying legacy:", modernError);

            try {
                await window.mapLoader.loadLegacy(apiKey);

                map = new google.maps.Map(el, {
                    center,
                    zoom: 15,
                    streetViewControl: false,
                    mapTypeControl: false,
                    fullscreenControl: false
                });

                marker = new google.maps.Marker({
                    position: center,
                    map: map,
                    draggable: true,
                    title: "Drag me to set location"
                });

                console.log("[mapPicker] Map and marker created (legacy)");

            } catch (legacyError) {
                console.error("[mapPicker] Both modern and legacy loads failed:", legacyError);
                el.innerHTML = '<div style="padding: 20px; text-align: center; color: #666;">Failed to load map. Please refresh the page.</div>';
                return;
            }
        }

        if (map && marker) {
            try {
                const observer = new ResizeObserver(() => {
                    if (google && google.maps && google.maps.event) {
                        google.maps.event.trigger(map, "resize");
                        map.setCenter(center);
                    }
                });
                observer.observe(el);
            } catch (e) {
                console.warn("[mapPicker] ResizeObserver not available");
            }

            map.addListener("click", (e) => {
                const cLat = e.latLng.lat();
                const cLng = e.latLng.lng();

                marker.setPosition({ lat: cLat, lng: cLng });

                dotNetHelper.invokeMethodAsync("OnMapClick", cLat, cLng);
            });

            marker.addListener("dragend", (e) => {
                const dLat = e.latLng.lat();
                const dLng = e.latLng.lng();
                dotNetHelper.invokeMethodAsync("OnMapClick", dLat, dLng);
            });

            console.log("[mapPicker] Map picker initialized successfully");
        }
    }

    return { init };
})();