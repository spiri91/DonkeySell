'use strict';

app.controller('editRegisterController',
['$scope', 'usersService', 'toastr', '$location', 'othersService', '$routeParams', '$q', editRegisterController]);

function editRegisterController($scope, usersService, toastr, $location, othersService, $routeParams, $q) {
    $scope.user = {};
    $scope.errors = [];
    $scope.confirmPassword = "";
    $scope.isEdit = false;
    $scope.isLoading = false;
    $scope.checking = false;

    $scope.extraCheck = function () {
        let promisses = [];

        if ($scope.isEdit === false) {
            promisses.push($scope.checkEmail());
            promisses.push($scope.checkUsername());
        }

        $scope.checkAddress();
        $scope.checkPassword();
        $scope.checkPasswordValidation();

        return $q.all(promisses);
    }

    $scope.register = function () {
        $scope.loading = true;
        $scope.extraCheck().then(() => {
            if ($scope.errors.length === 0)
                usersService.createEditUser($scope.user)
                    .then(function (user) {
                        if (user.data) {
                            if (user.data.userName === $scope.user.userName) {
                                let message = $scope.isEdit ? "Edit successful" : "Register successful";
                                toastr.success(message);
                                $location.path('/home');
                                $scope.$parent.login();
                            }
                        }
                    },
                        function (error) {
                            toastr.error('Plase try again!');
                            $scope.doSomethingWithError(error);
                        });
            else
                $scope.loading = false;
        });
    };

    $scope.checkAddress = function () {
        let address = $scope.user.address;
        if (address === undefined || address.length < 8 || address.length > 50) {
            $scope.addError(new ErrorTypeAndValue("addressLength", "Address should be between 8 and 30 characters!"));
            return;
        }

        $scope.removeErrorIfExists("addressLength");
    }

    $scope.checkUsername = function () {
        $scope.checking = true;
        if ($scope.user.userName === undefined || $scope.user.userName.length < 4) {
            $scope.addError(new ErrorTypeAndValue("usernameFormat", "Username should be between 5 and 15 characters!"));
            return null;
        }

        $scope.removeErrorIfExists("usernameFormat");

       return othersService.usernameIsTaken($scope.user.userName)
            .then(function (taken) {
                let usernameIsTaken = taken.data;
                if (usernameIsTaken === true)
                    $scope.addError(new ErrorTypeAndValue("usernameError", "This username is allready taken!"));
                else {
                    $scope.removeErrorIfExists("usernameError");
                }
            }, function (error) {
                console.log(error);
            }).finally(() => { $scope.checking = false; });
    }

    $scope.checkEmail = function () {
        $scope.checking = false;
        if (!$scope.checkMailIfValidFormat()) {
            $scope.addError(new ErrorTypeAndValue("emailFormatError", "This email is not valid!"));
            return null;
        } else {
            $scope.removeErrorIfExists("emailFormatError");
        }

        if (!$scope.isEdit)
            return othersService.emailInUse($scope.user.email)
                  .then(function (inUse) {
                      let emailIsTaken = inUse.data;
                      if (emailIsTaken === true)
                          $scope.addError(new ErrorTypeAndValue("emailError", "This email is allready in use!"));
                      else {
                          $scope.removeErrorIfExists("emailError");
                      }
                  }, function (error) {
                      $scope.doSomethingWithError(error);
                  }).finally(() => { $scope.checking = false; });
    };

    $scope.checkMailIfValidFormat = function () {
        if ($scope.user.email !== undefined && $scope.user.email.indexOf('@') > 0 && $scope.user.email.indexOf('.') > 0)
            return true;

        return false;
    }

    $scope.checkPassword = function () {
        let passwordIsValidRegExp = new RegExp(/^([a-zA-Z0-9_-]){8,16}$/);
        let passwordIsValid = passwordIsValidRegExp.exec($scope.user.password);
        if (passwordIsValid === null)
            $scope.addError(new ErrorTypeAndValue("passwordError",
                "Password should be between 8 and 16 characters, should contain at least one digit, one uppercase letter no symbol characters!"));
        else {
            $scope.removeErrorIfExists("passwordError");
        }
    }

    $scope.checkPasswordValidation = function () {
        if ($scope.confirmPassword !== $scope.user.password)
            $scope.addError(new ErrorTypeAndValue("confirmPasswordError",
                "Password doesn't match the confirm password field!"));
        else {
            $scope.removeErrorIfExists("confirmPasswordError");
        }
    }

    $scope.cancel = function () {
        $location.path('/home');
    };

    $scope.removeErrorIfExists = function (errorType) {
        let error = $scope.errors.filter(function (x) { return x.type === errorType })[0];
        if (error) {
            let index = $scope.errors.indexOf(error);
            $scope.errors.splice(index, 1);
        }
    }

    $scope.addError = function (error) {
        $scope.removeErrorIfExists(error.type);
        $scope.errors.push(error);
    };

    $scope.init = function () {
        if ($routeParams.username) {
            $scope.isEdit = true;
            usersService.getUser($routeParams.username)
                .then(function (user) {
                    if (user.data)
                        $scope.user = user.data;
                }, function (error) {
                    toastr.error('Plase try again!');
                    $scope.doSomethingWithError(error);
                });
        } else {
            $scope.user = new User();
        }
    };

    $scope.setAvatar = function () {
        if ((document.getElementById('pictures')).files[0]) {
            let file = (document.getElementById('pictures')).files[0];
            let reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = function () {
                $scope.$apply(function () {
                    $scope.user.avatar = reader.result;
                });
            }
        }
    }

    $scope.doSomethingWithError = function (error) {
        console.log(error);
    };

    $scope.init();

    // ReSharper disable once InconsistentNaming
    function ErrorTypeAndValue(type, value) {
        this.type = type;
        this.value = value;
    };
}