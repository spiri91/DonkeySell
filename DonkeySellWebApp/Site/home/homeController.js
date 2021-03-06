﻿app.controller('homeController', ['$scope', 'usersService', '$location', 'productsService', '$uibModal','sortOptionsService','productNavigationService', homeController]);

function homeController($scope, usersService, $location, productsService, $uibModal, sortOptionsService, productNavigationService) {
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
            $('#searchBtn').focus();
        });
    };

    $scope.setProductNavigation = function () {
        let productsIds = $scope.products.map(function (product) { return product.id; });
        let sortBy = sortOptionsService.getSortOptions()[2];
        productNavigationService.setValues("all", 0, sortBy, 6, productsIds);
    }

    $scope.showProduct = function (id) {
        $scope.setProductNavigation();
        $location.url('/product/' + id);
    };

    $scope.init();
}