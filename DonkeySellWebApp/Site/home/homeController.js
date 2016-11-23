app.controller('homeController', ['$scope','usersService', '$location', 'productsService', homeController]);

function homeController($scope, usersService, $location, productsService) {
    $scope.productName = "";
    $scope.selectedCityId = 0;
    $scope.products = [];

    $scope.simulateQuery = false;
    $scope.isDisabled = false;
    $scope.searchText = "";

    $scope.searchSpecific = function () {
        if ($scope.productName === "")
            $scope.productName = 'allProducts';
        if ($scope.selectedCityId === 0)
            $scope.selectedCityId = 'allCities';
        this.getProducts($scope.productName, $scope.selectedCityId);
    };

    $scope.showAll = function() {
        this.getProducts('allProducts', 'allCities');
    };

    $scope.getProducts = function (products, city) {
        $location.url('/products/'+ products +'/'+ city );
    }

    $scope.init = function () {
        productsService.queryProducts("all", 6, 0, "DatePublished", sortDirection.descending)
            .then(function (productsAndCount) {
                if (productsAndCount.data)
                    $scope.products = productsAndCount.data.products;
            }, function() {
                console.log(error);
            });
    };

    $scope.init();
}