window.mapPicker = (function () {
    let mapInstance = null;
    let markerInstance = null;

    function waitForGoogleMaps() {
        return new Promise((resolve, reject) => {
            if (window.google && window.google.maps) {
                resolve();
                return;
            }
            let attempts = 0;
            const interval = setInterval(() => {
                attempts++;
                if (window.google && window.google.maps) {
                    clearInterval(interval);
                    resolve();
                } else if (attempts > 50) { 
                    clearInterval(interval);
                    reject("Google Maps failed to load.");
                }
            }, 100);
        });
    }

    function loadGoogleMaps(apiKey) {
        if (window.google && window.google.maps) return Promise.resolve();

        const existingScript = document.querySelector('script[src*="maps.googleapis.com/maps/api/js"]');
        if (existingScript) {
            return waitForGoogleMaps();
        }

        return new Promise((resolve, reject) => {
            const script = document.createElement("script");
            script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&v=quarterly&loading=async`;
            script.async = true;
            script.defer = true;
            script.onload = () => resolve();
            script.onerror = (err) => reject(err);
            document.head.appendChild(script);
        });
    }

    async function init(elementId, apiKey, lat, lng, dotNetHelper) {
        try {
            await loadGoogleMaps(apiKey);

            const safeLat = (lat === 0 && lng === 0) ? 42.6977 : lat;
            const safeLng = (lat === 0 && lng === 0) ? 23.3219 : lng;
            const defaultLocation = { lat: safeLat, lng: safeLng };

            const mapElement = document.getElementById(elementId);
            if (!mapElement) return;

            mapInstance = new google.maps.Map(mapElement, {
                center: defaultLocation,
                zoom: 15,
                streetViewControl: false,
                mapTypeControl: false,
                fullscreenControl: false
            });

            markerInstance = new google.maps.Marker({
                position: defaultLocation,
                map: mapInstance,
                draggable: true,
                title: "Drag to select location"
            });

            mapInstance.addListener("click", (e) => {
                const clickedLat = e.latLng.lat();
                const clickedLng = e.latLng.lng();
                markerInstance.setPosition({ lat: clickedLat, lng: clickedLng });
                dotNetHelper.invokeMethodAsync("OnMapClick", clickedLat, clickedLng);
            });

            markerInstance.addListener("dragend", (e) => {
                const draggedLat = e.latLng.lat();
                const draggedLng = e.latLng.lng();
                dotNetHelper.invokeMethodAsync("OnMapClick", draggedLat, draggedLng);
            });

        } catch (error) {
            console.error("[mapPicker] Initialization failed:", error);
        }
    }

    return { init };
})();