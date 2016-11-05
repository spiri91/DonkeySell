﻿app.controller('loginController', ['$scope', '$uibModalInstance', 'usersService', 'toastr', 'storageService', loginController]);

function loginController($scope, $uibModalInstance, usersService, toastr, storageService) {
    $scope.username = "";
    $scope.password = "";
    $scope.rememberCredentials = false;
    var passwordCacheName = "password";
    var usernameCacheName = "username";

    $scope.login = function () {
        let user = new User();
        user.userName = $scope.username;
        user.password = $scope.password;
        usersService.logIn(user)
            .then(function () {
                toastr.success("Login Successful!");
                $scope.rememberCredentialsOption();
                $uibModalInstance.close($scope.username);
            },
                function () {
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
        storageService.set(usernameCacheName, $scope.username);
        storageService.set(passwordCacheName, $scope.password);
    };

    $scope.rememberCredentialsOption = function () {
        if ($scope.rememberCredentials === true)
            $scope.setCredentialsInMemory();
        else
            $scope.removeCredentialsFromMemory();
    };

    $scope.init();
}