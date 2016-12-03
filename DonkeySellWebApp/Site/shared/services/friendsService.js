app.service('friendsService', ['apiRootAddress', '$http', friendsService]);

function friendsService(apiRootAddress, $http) {
    this.composeAddress = function (username) {
        return apiRootAddress + "Users/" + username + "/Friends";
    }

    this.getFriends = function(username, token) {
        return $http({
            method: 'GET',
            url: this.composeAddress(username),
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.postFriend = function(username, friend, token) {
        return $http({
            method: 'POST',
            url: this.composeAddress(username) + "/" + friend,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.deleteFriend = function(username, friend, token) {
        return $http({
            method: 'DELETE',
            url: this.composeAddress(username) + "/" + friend,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    return this;
};