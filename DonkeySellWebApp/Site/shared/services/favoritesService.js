app.service('favoritesService', ['apiRootAddress', '$http', favoritesService]);

function favoritesService(apiRootAddress, $http) {
    this.composeAddress = function (username) {
        return apiRootAddress + "Users/" + username + "/Favorites";
    }

    this.getFavoritesProducts = function(username, token) {
        return $http({ method: 'GET', url: this.composeAddress(username), headers: { 'Authorization': 'Bearer ' + token } })
            .then(this.successHandler, this.errorHandler);
    }

    this.postProductToFavorites = function(username, token, productId) {
        return $http({ method: 'POST', url: this.composeAddress(username)+'/'+productId, headers: { 'Authorization': 'Bearer ' + token }})
            .then(this.successHandler, this.errorHandler);
    }

    this.deleteFromFavoritesProducts = function(username, token, productId) {
        return $http({ method: 'DELETE', url: this.composeAddress(username) + '/' + productId, headers: { 'Authorization': 'Bearer ' + token }})
            .then(this.successHandler, this.errorHandler);
    }

    this.successHandler = function (data) {
        return data;
    }

    this.errorHandler = function (error) {
        console.log(error.message);
    }

    return this;
}