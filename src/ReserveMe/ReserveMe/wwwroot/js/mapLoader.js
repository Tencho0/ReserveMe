window.mapLoader = (function () {
    let modernLoadPromise = null;
    let legacyLoadPromise = null;
    let currentApiKey = null;

    const originalOnError = window.onerror;
    window.onerror = function (msg) {
        if (msg && (
            msg.toString().includes("reading 'zJ'") ||
            msg.toString().includes("reading 'DJ'") ||
            msg.toString().includes("IntersectionObserver")
        )) {
            return true; 
        }
        if (originalOnError) return originalOnError.apply(this, arguments);
    };

    function injectScript(src) {
        return new Promise((resolve, reject) => {
            const exists = Array.from(document.scripts).some(s => {
                return s.src === src ||
                    (s.src.includes("maps.googleapis.com") && s.src.includes("key="));
            });

            if (exists) {
                console.log("[mapLoader] Script already exists:", src);
                resolve();
                return;
            }

            console.log("[mapLoader] Injecting script:", src);
            const script = document.createElement("script");
            script.src = src;
            script.async = true;
            script.defer = true;
            script.onload = () => {
                console.log("[mapLoader] Script loaded successfully");
                resolve();
            };
            script.onerror = () => {
                console.error("[mapLoader] Script failed to load");
                reject(new Error("Failed to load Google Maps API."));
            };
            document.head.appendChild(script);
        });
    }

    function loadModern(apiKey) {
        if (!apiKey || typeof apiKey !== "string" || apiKey.trim() === "") {
            return Promise.reject(new Error("Google Maps API key is missing."));
        }

        if (window.google &&
            window.google.maps &&
            typeof google.maps.importLibrary === "function" &&
            currentApiKey === apiKey) {
            console.log("[mapLoader] Modern API already loaded");
            return Promise.resolve();
        }

        if (modernLoadPromise && currentApiKey === apiKey) {
            return modernLoadPromise;
        }

        currentApiKey = apiKey;
        const src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}&v=quarterly&loading=async`;
        modernLoadPromise = injectScript(src)
            .then(() => {
                return new Promise(resolve => setTimeout(resolve, 100));
            })
            .then(() => {
                if (!window.google || !window.google.maps) {
                    throw new Error("Google Maps API did not initialize properly");
                }
            });

        return modernLoadPromise;
    }

    function loadLegacy(apiKey) {
        if (!apiKey || typeof apiKey !== "string" || apiKey.trim() === "") {
            return Promise.reject(new Error("Google Maps API key is missing."));
        }

        if (window.google &&
            window.google.maps &&
            typeof google.maps.Map === "function" &&
            currentApiKey === apiKey) {
            console.log("[mapLoader] Legacy API already loaded");
            return Promise.resolve();
        }

        if (legacyLoadPromise && currentApiKey === apiKey) {
            return legacyLoadPromise;
        }

        currentApiKey = apiKey;
        const src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}&v=quarterly`;
        legacyLoadPromise = injectScript(src)
            .then(() => {
                return new Promise(resolve => setTimeout(resolve, 100));
            })
            .then(() => {
                if (!window.google || !window.google.maps) {
                    throw new Error("Google Maps API did not initialize properly");
                }
            });

        return legacyLoadPromise;
    }

    return {
        loadModern,
        loadLegacy,
        isLoaded: () => {
            return !!(window.google && window.google.maps);
        },
        reset: () => {
            modernLoadPromise = null;
            legacyLoadPromise = null;
            currentApiKey = null;
        }
    };
})();