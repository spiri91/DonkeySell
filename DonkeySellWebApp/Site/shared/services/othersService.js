app.service('othersService', ['apiRootAddress', '$http', othersService]);

function othersService(apiRootAddress, $http) {
    this.urlApiAddress = apiRootAddress + "Other";

    this.getCategories = function() {
        let urlAddress = this.urlApiAddress + "/categories";

        return $http.get(urlAddress).then(this.successHandler, this.errorHandler);
    }

    this.getCities = function() {
        let urlAddress = this.urlApiAddress + "/cities";

        return $http.get(urlAddress).then(this.successHandler, this.errorHandler);
    }

    this.usernameIsTaken = function(username) {
        let urlAddress = this.urlApiAddress + "/usernameIsNotTaken/" + username;

        return $http.get(urlAddress).then(this.successHandler, this.errorHandler);
    }

    this.emailInUse = function (email) {
        email = email.replace('.', '$');
        let urlAddress = this.urlApiAddress + "/emailNotInUse/" + email;

        return $http.get(urlAddress, email).then(this.successHandler, this.errorHandler);
    };

    this.successHandler = function (data) {
        return data;
    }

    this.errorHandler = function (error) {
        console.log(error.message);
    }

    return this;
}