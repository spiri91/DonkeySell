app.service('storageService', [storageService]);

function storageService() {
    this.set = function(key, value) {
        localStorage.setItem(key, value);
    };

    this.get = function(key) {
        return localStorage.getItem(key);
    };

    this.remove = function(key) {
        localStorage.removeItem(key);
    };

    return this;
}