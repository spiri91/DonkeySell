app.controller('productsController', ['$scope', 'productsService', '$routeParams', '$location', 'queryBuilderService', 'toastr','sortOptionsService', productsController]);

function productsController($scope, productsService, $routeParams, $location, queryBuilderService, toastr,sortOptionsService) {
    $scope.productName = $routeParams.productName;
    $scope.cityId = $routeParams.city;
    $scope.skip = 0;
    $scope.query = "all";
    $scope.itemsPerPage = 6;
    $scope.sortOptions = sortOptionsService.getSortOptions();
    $scope.sortBy = {};
    $scope.count = 0;
    $scope.currentPage = 0;
    $scope.totalPages = 0;

    $scope.products = [];

    $scope.init = function () {
        $scope.sortBy = $scope.sortOptions[2];
        let queryParts = new Array();
        if ($scope.productName !== "allProducts") {
            let queryPartTitle = new QueryBuilderPart('title', $scope.productName, $scope.productName, true);
            queryParts.push(queryPartTitle);
        }

        if ($scope.cityId !== "allCities") {
            let queryPartCity = new QueryBuilderPart('cityId', $scope.cityId, $scope.cityId, null);
            queryParts.push(queryPartCity);
        }

        if(queryParts.length > 0)
        $scope.query = queryBuilderService.buildQuery(queryParts);

        $scope.getProducts();
    };

    $scope.getProducts = function () {
        $scope.itemsPerPage = $scope.itemsPerPage ? $scope.itemsPerPage : 4;
        
        productsService.queryProducts($scope.query, $scope.itemsPerPage, $scope.skip, $scope.sortBy.value, $scope.sortBy.sortDirection)
                .then(function (productsAndCount) {
                if (productsAndCount.data) {
                    $scope.products = productsAndCount.data.products;
                    $scope.count = productsAndCount.data.count;
                    $scope.calculatePages();
                }
            }, function (error) {
                    toastr.error('An error occured!');
                    $scope.doSomethingWithError(error);
                });
    }

    $scope.resetGetProducts = function () {
        $scope.skip = 0;
        $scope.getProducts();
    }

    $scope.getProduct = function (id) {
        $location.url('/product/' + id);
    }

    $scope.showNextProducts = function () {
        $scope.skip += $scope.itemsPerPage;
        $scope.getProducts();
    };

    $scope.calculatePages = function () {
        let totalPages = $scope.count / $scope.itemsPerPage;
        $scope.totalPages = Math.ceil(totalPages);

        if ($scope.skip === 0)
            $scope.currentPage = 1;
        else {
            let currentPage = $scope.skip / $scope.itemsPerPage;
            $scope.currentPage = Math.floor(currentPage) + 1;
        }
    } 

    $scope.showPreviousProducts = function () {
        if ($scope.skip !== 0) {
            $scope.skip -= $scope.itemsPerPage;
            $scope.getProducts();
        }
    }

    $scope.doSomethingWithError = function (error) {
        console.log(error);
    }

    $scope.init();
}