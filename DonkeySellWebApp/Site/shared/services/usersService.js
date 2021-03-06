﻿app.service('usersService', ['apiRootAddress', '$http', 'storageService', usersService]);

function usersService(apiRootAddress, $http, storageService) {
    this.tokenApiAddress = apiRootAddress.substring(0, apiRootAddress.length - 4) + "Token";
    this.usersApiAdress = apiRootAddress + "Users";

    this.logIn = function (user) {
        let getTokenDataString = "grant_type=password&username=" + user.userName + "&password=" + user.password;

        return $.ajax({
            method: 'POST',
            url: this.tokenApiAddress,
            headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
            data: getTokenDataString
        }).then(function (data) {
            storageService.set('token', data.access_token);
            return true;
        }).fail(
                function (error) {
                    console.log(error.message);
                    return false;
                });
    };

    this.signOut = function () {
        storageService.remove('token');
        return true;
    }

    this.getUser = function (username) {
        let getUserAddress = this.usersApiAdress + "/" + username;

        return $http({ method: 'GET', url: getUserAddress });
    }

    this.createEditUser = function (user) {
        return $http.post(this.usersApiAdress, user);
    }

    this.deleteUser = function (username, token) {
        return $http({
            method: 'DELETE',
            url: this.usersApiAdress + "/" + username,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.getUnreadMessages = function (username, token) {
        return $http({
            method: 'GET',
            url: this.usersApiAdress + "/" + username + "/unread",
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.markMessageRead = function (username, token, messageId) {
        return $http({
            method: 'POST',
            url: this.usersApiAdress + "/" + username + "/MarkRead/" + messageId,
            headers: { 'Authorization': 'Bearer ' + token }
        });
    }

    this.changePassword = function (username, resetPassword) {
        return $http.post(this.usersApiAdress + "/" + username + "/changePassword", resetPassword);
    }

    this.resetPassword = function (username) {
        return $http.post(this.usersApiAdress + "/" + username + "/resetPassword");
    }

    this.getToken = function () {
        return storageService.get('token');
    }

    return this;
}