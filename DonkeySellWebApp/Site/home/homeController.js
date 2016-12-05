﻿app.controller('homeController', ['$scope', 'usersService', '$location', 'productsService', '$uibModal', homeController]);

function homeController($scope, usersService, $location, productsService, $uibModal) {
    $scope.productName = "";
    $scope.selectedCity = {};
    $scope.products = [];
    $scope.loading = false;

    $scope.simulateQuery = false;
    $scope.isDisabled = false;
    $scope.searchText = "";

    $scope.searchSpecific = function () {
        if ($scope.productName === "")
            $scope.productName = 'allProducts';
        if (!$scope.selectedCity.id || $scope.selectedCity.id === 0)
            $scope.selectedCity.id = 'allCities';
        this.getProducts($scope.productName, $scope.selectedCity.id);
    };

    $scope.showAll = function () {
        this.getProducts('allProducts', 'allCities');
    };

    $scope.getProducts = function (products, city) {
        $location.url('/products/' + products + '/' + city);
    }

    $scope.init = function () {
        $scope.loading = true;
        productsService.queryProducts("all", 6, 0, "DatePublished", sortDirection.descending)
            .then(function(productsAndCount) {
                    if (productsAndCount.data)
                        $scope.products = productsAndCount.data.products;
                },
                function() {
                    console.log(error);
                })
            .finally(function() {
                $scope.loading = false;
            });
    };

    $scope.openCitiesModal = function () {
        let modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Site/choseCity/choseCity.html',
            controller: 'choseCityController',
            resolve: {
                cities: function () { return $scope.$parent.cities }
            }
        });

        modalInstance.result.then(function (selectedCity) {
            $scope.selectedCity = selectedCity;
        });
    };

    $scope.showProduct = function (id) {
        $location.url('/product/' + id);
    };

    $scope.init();
}