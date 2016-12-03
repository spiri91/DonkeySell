app.service('improvementsService', ['apiRootAddress', '$http', improvementsService]);

function improvementsService(apiRootAddress, $http) {
    this.fullApiAddress = apiRootAddress + "Improvements";

    this.postImprovement =function(improvement) {
        return $http.post(this.fullApiAddress, improvement);
    }

    return this;
};