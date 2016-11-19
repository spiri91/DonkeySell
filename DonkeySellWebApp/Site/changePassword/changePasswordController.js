app.controller('changePasswordController', ['$scope', '$uibModalInstance', 'usersService', '$routeParams', 'toastr', 'username', changePasswordController]);

function changePasswordController($scope, $uibModalInstance, usersService, $routeParams, toastr, username) {
    $scope.username = username;
    $scope.oldPassword = "";
    $scope.newPassword = "";
    $scope.confirmPassword = "";
    $scope.showError = false;
    $scope.showPasswordConfirmationError = false;

    $scope.checkPassword = function () {
        let passwordIsValidRegExp = new RegExp(/^([a-zA-Z0-9_-]){8,16}$/);
        let passwordIsValid = passwordIsValidRegExp.exec($scope.newPassword);
        $scope.showError = passwordIsValid === null ? true : false;
    }

    $scope.changePassword = function() {
        let resetPasswordModel = new ResetPassword($scope.oldPassword, $scope.newPassword);
        usersService.changePassword($scope.username, resetPasswordModel)
            .then(function () {
                toastr.success('Password was changed!');
                $scope.cancel();
            }, function() {
                toastr.error("Change password failed!");
            });

    };

    $scope.checkPasswordConfirmation = function() {
        $scope.showPasswordConfirmationError = $scope.newPassword === $scope.confirmPassword ? false : true;
    }

    $scope.cancel = function() {
        $uibModalInstance.close();
    }
};