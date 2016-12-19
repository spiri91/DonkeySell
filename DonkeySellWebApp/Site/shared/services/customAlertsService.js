app.service('customAlertsService', ['apiRootAddress', '$http', customAlertsService]);

function customAlertsService(apiRootAddress, $http) {
    this.composeAddress = function(username) {
        return apiRootAddress + "Users/" + username + "/Alerts";
    };

    this.getAlerts = function(username, token) {
        return $http({
            method: 'GET',
            url: this.composeAddress(username),
            headers: { 'Authorization': 'Bearer ' + token }
        });
    };

    this.postAlert = function (customAlertsService, token, username) {
        return $http({
            method: 'POST',
            url: this.composeAddress(username),
            headers: { 'Authorization': 'Bearer ' + token },
            data: customAlertsService
        });
    };

    this.deleteAlert = function(id, token, username) {
        return $http({
            method: 'DELETE',
            url: this.composeAddress(username) + "/" + id,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    };
}