window.reserveMeGeo = {
    getCurrentPosition: function (options) {
        return new Promise((resolve, reject) => {
            if (!('geolocation' in navigator)) {
                reject({ code: -1, message: 'Geolocation is not supported by this browser.' });
                return;
            }
            navigator.geolocation.getCurrentPosition(
                (pos) => {
                    resolve({
                        lat: pos.coords.latitude,
                        lon: pos.coords.longitude,
                        accuracy: pos.coords.accuracy
                    });
                },
                (err) => reject({ code: err.code, message: err.message }),
                options
            );
        });
    }
};