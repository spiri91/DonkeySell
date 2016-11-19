app.service('improvementsService', ['apiRootAddress', '$http', improvementsService]);

function improvementsService(apiRootAddress, $http) {
    this.fullApiAddress = apiRootAddress + "Improvements";

    this.postImprovement =function(improvement) {
        return $http.post(this.fullApiAddress, improvement).then(this.successHandler, this.errorHandler);
    }

    this.successHandler = function (data) {
        return data;
    }

    this.errorHandler = function (error) {
        console.log(error.message);
    }

    return this;
};