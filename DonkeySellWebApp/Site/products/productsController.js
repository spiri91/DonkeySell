﻿app.controller('productsController', ['$scope', 'productsService', '$routeParams', '$location', 'queryBuilderService', productsController]);

function productsController($scope, productsService, $routeParams, $location, queryBuilderService) {
    $scope.productName = $routeParams.productName;
    $scope.cityId = $routeParams.city;
    $scope.skip = 0;
    $scope.query = "all";
    $scope.itemsPerPage = 6;
    $scope.sortBy = "DatePublished";
    $scope.sortOptions = ["Price", "DatePublished", "UserName"];

    $scope.products = [];

    $scope.init = function () {
        let queryParts = new Array();
        if ($scope.productName !== "allProducts" || $scope.cityId !== "allCities") {
            if ($scope.productName !== '*') {
                let queryPartTitle = new QueryBuilderPart('title', $scope.productName, $scope.productName, true);
                queryParts.push(queryPartTitle);
            }

            let queryPartCity = new QueryBuilderPart('cityId', $scope.cityId, $scope.cityId, null);
            queryParts.push(queryPartCity);
            $scope.query = queryBuilderService.buildQuery(queryParts);
        }

        $scope.getProducts();
    };

    $scope.getProducts = function () {
        $scope.itemsPerPage = $scope.itemsPerPage ? $scope.itemsPerPage : 4;
        productsService.queryProducts($scope.query, $scope.itemsPerPage, $scope.skip, $scope.sortBy)
                .then(function (products) {
                    $scope.products = products.data;
                });
    }

    $scope.resetGetProducts = function() {
        $scope.skip = 0;
        $scope.getProducts();
    }

    $scope.getProduct = function (id) {
        $location.url('/product/' + id);
    }

    $scope.showNextProducts = function() {
        $scope.skip += $scope.itemsPerPage;
        $scope.getProducts();
    };

    $scope.showPreviousProducts = function () {
        if ($scope.skip !== 0) {
            $scope.skip -= $scope.itemsPerPage;
            $scope.getProducts();
        }
    }

    $scope.init();
}