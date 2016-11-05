app.controller('homeController', ['$scope','usersService', '$location', 'productsService', homeController]);

function homeController($scope, usersService, $location, productsService) {
    $scope.productName = "";
    $scope.selectedCityId = 10;
    $scope.products = [];

    $scope.simulateQuery = false;
    $scope.isDisabled = false;
    $scope.searchText = "";

    $scope.searchSpecific = function () {
        if ($scope.productName === "")
            $scope.productName = '*';
        this.getProducts($scope.productName, $scope.selectedCityId);
    };

    $scope.showAll = function() {
        this.getProducts('allProducts', 'allCities');
    };

    $scope.getProducts = function (products, city) {
        $location.url('/products/'+ products +'/'+ city );
    }

    $scope.init = function () {
        productsService.getTopProducts()
            .then(function(products) {
                $scope.products = products.data;
            });
    };

    $scope.init();
}