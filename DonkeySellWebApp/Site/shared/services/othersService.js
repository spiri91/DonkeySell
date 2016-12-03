app.service('othersService', ['apiRootAddress', '$http', othersService]);

function othersService(apiRootAddress, $http) {
    this.urlApiAddress = apiRootAddress + "Other";

    this.getCategories = function() {
        let urlAddress = this.urlApiAddress + "/categories";

        return $http.get(urlAddress);
    }

    this.getCities = function() {
        let urlAddress = this.urlApiAddress + "/cities";

        return $http.get(urlAddress);
    }

    this.usernameIsTaken = function(username) {
        let urlAddress = this.urlApiAddress + "/usernameIsNotTaken/" + username;

        return $http.get(urlAddress);
    }

    this.emailInUse = function (email) {
        let emailWithoutDots = email.replace(/\./g, '$');
        let urlAddress = this.urlApiAddress + "/emailNotInUse/" + emailWithoutDots;

        return $http.get(urlAddress);
    };

    this.usersLike = function(username) {
        let urlAddress = this.urlApiAddress + "/findUser/" + username;

        return $http.get(urlAddress);
    }

    return this;
}