'use strict';

app.controller('registerController', ['$scope', 'usersService', 'toastr', '$location', 'othersService', registerController]);

function registerController($scope, usersService, toastr, $location, othersService) {
    $scope.user = new User();
    $scope.inProgress = false;
    $scope.errors = [];
    $scope.confirmPassword = "";

    $scope.signIn = function () {
        usersService.createNewUser($scope.user)
            .then(function () {
                toastr.success('Register successful!');
                $location.path('/home');
            });
    };

    $scope.checkUsername = function () {
        if ($scope.user.email)
        othersService.usernameIsTaken($scope.user.userName)
            .then(function (taken) {
                let usernameIsNotTaken = taken.data;
                if (usernameIsNotTaken === false)
                    $scope.addError(new ErrorTypeAndValue("usernameError", "This username is allready taken!"));
                else {
                    $scope.removeErrorIfExists("usernameError");
                }
            });
    }

    $scope.checkEmail = function () {
        if ($scope.user.email !== undefined && $scope.user.email.indexOf('@')>0 && $scope.user.email.indexOf('.')>0)
        othersService.emailInUse($scope.user.email)
            .then(function(inUse) {
                let emailInUse = inUse.data;
                if (emailInUse === false)
                    $scope.addError(new ErrorTypeAndValue("emailError", "This email is allready in use!"));
                else {
                    $scope.removeErrorIfExists("emailError");
                }
            });
    };

    $scope.checkPassword = function () {
        let passwordIsValidRegExp = new RegExp(/^([a-zA-Z0-9_-]){8,16}$/);
        let passwordIsValid = passwordIsValidRegExp.exec($scope.user.password);
        if (passwordIsValid === null)
            $scope.addError(new ErrorTypeAndValue("passwordError", "Password should be between 8 and 16 characters, should contain at least one digit, one uppercase letter no symbol characters!"));
        else {
            $scope.removeErrorIfExists("passwordError");
        }
    }

    $scope.checkPasswordValidation = function () {
        if ($scope.confirmPassword !== $scope.user.password)
            $scope.addError(new ErrorTypeAndValue("confirmPasswordError", "Password doesn't match the confirm password field!"));
        else {
            $scope.removeErrorIfExists("confirmPasswordError");
        }
    }

    $scope.cancel = function () {
        $location.path('/home');
    };

    $scope.removeErrorIfExists = function (errorType) {
        let error = $scope.errors.filter(function (x) {return x.type === errorType })[0];
        if (error) {
            let index = $scope.errors.indexOf(error);
            $scope.errors.splice(index, 1);
        }
    }

    $scope.addError = function (error) {
        $scope.removeErrorIfExists(error.type);
        $scope.errors.push(error);
    }

    // ReSharper disable once InconsistentNaming
    function ErrorTypeAndValue(type, value) {
        this.type = type;
        this.value = value;
    };
}