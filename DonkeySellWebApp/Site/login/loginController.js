﻿app.controller('loginController', ['$scope', '$uibModalInstance', 'usersService', 'toastr', 'storageService', '$location', loginController]);

function loginController($scope, $uibModalInstance, usersService, toastr, storageService, $location) {
    $scope.username = "";
    $scope.password = "";
    $scope.rememberCredentials = false;
    $scope.mailSent = false;
    $scope.isLoading = false;

    var passwordCacheName = "password";
    var usernameCacheName = "username";

    $scope.login = function () {
        $scope.isLoading = true;
        let user = new User();
        user.userName = $scope.username;
        user.password = $scope.password;
        usersService.logIn(user)
            .then(function () {
                toastr.success("Login successful!");
                $scope.rememberCredentialsOption();
                storageService.set(usernameCacheName, $scope.username);
                $uibModalInstance.close($scope.username);
            },
                function () {
                    $scope.isLoading = false;
                    toastr.error('Login failed!');
                    $scope.username = $scope.password = "";
                });
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.init = function () {
        $scope.username = storageService.get(usernameCacheName);
        $scope.password = storageService.get(passwordCacheName);
        if ($scope.username && $scope.password)
            $scope.rememberCredentials = true;
    };

    $scope.removeCredentialsFromMemory = function () {
        storageService.remove(passwordCacheName);
    };

    $scope.setCredentialsInMemory = function () {
        storageService.set(passwordCacheName, $scope.password);
    };

    $scope.rememberCredentialsOption = function () {
        if ($scope.rememberCredentials === true)
            $scope.setCredentialsInMemory();
        else
            $scope.removeCredentialsFromMemory();
    };

    $scope.resetPassword = function () {
        if ($scope.username)
            usersService.resetPassword($scope.username)
                .then(function () {
                    $scope.mailSent = true;
                },
                    function (error) {
                        console.log(error);
                        toastr.error('Reset password failed! :(');
                    });
    };

    $scope.register = function () {
        $scope.cancel();
        $location.url('/register');
    };

    $scope.init();
}