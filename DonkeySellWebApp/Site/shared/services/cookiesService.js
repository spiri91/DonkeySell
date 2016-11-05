app.service('cookiesService', ['$cookies', cookiesService]);

function cookiesService($cookies) {
    this.setCookie =  function(key, value) {
        return $cookies.put(key, value);
    }

    this.getCookie = function(key) {
        return $cookies.get(key);
    }

    this.deleteCookie = function(key) {
        $cookies.remove(key);
    }

    return this;
}