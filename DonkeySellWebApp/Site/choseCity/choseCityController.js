app.controller("choseCityController", ['$scope', '$uibModalInstance', 'cities', choseCityController]);

function choseCityController($scope, $uibModalInstance, cities) {
    $scope.cities = cities;

    $scope.ok = function () {
        $uibModalInstance.close($scope.selectedCity);
    };

    $scope.selectCity = function (city) {
        if (!city) {
            city = new City(0, 'All');
        }

        $uibModalInstance.close(city);
    }

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

}