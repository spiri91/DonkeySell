app.controller('customAlertsController', ['$scope', 'customAlertsService', 'toastr', customAlertsController]);

function customAlertsController($scope, customAlertsService, toastr) {
    $scope.customAlerts = [];
    $scope.productName = '';
    $scope.user = $scope.$parent.user;
    $scope.token = $scope.$parent.token;

    $scope.init = function () {
        customAlertsService.getAlerts($scope.user.userName, $scope.token)
            .then(function (alerts) {
                if (alerts.data)
                    $scope.customAlerts = alerts.data;
            }, function (error) {
                console.log(error);
            });
    };

    $scope.addNewCustomAlert = function () {
        let customAlert = new CustomAlert();
        customAlert.userId = $scope.user.userId;
        customAlert.productName = $scope.productName;

        customAlertsService.postAlert(customAlert, $scope.token, $scope.user.userName)
            .then(function (customAlert) {
                if (customAlert.data)
                    $scope.customAlerts.push(customAlert.data);

                $scope.productName = '';
            }, function (error) {
                toastr.error('An error ocurrend! :(');
                console.log(error);
            });
    };

    $scope.deleteCustomAlert = function (id) {
        customAlertsService.deleteAlert(id, $scope.token, $scope.user.userName)
            .then(function (deletedId) {
                    if (deletedId.data) {
                        toastr.success('Alert removed!');
                        let alert = $scope.customAlerts.filter(function(x) { return x.id === deletedId.data; })[0];
                        let index = $scope.customAlerts.indexOf(alert);
                        $scope.customAlerts.splice(index, 1);
                    }
                },
                function (error) {
                    console.log(error);
                });
    }

    $scope.init();
}