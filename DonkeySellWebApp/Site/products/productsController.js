app.controller('productsController', ['$scope', 'productsService', '$routeParams', '$location', 'queryBuilderService',
    'toastr', 'sortOptionsService', '$rootScope', 'productsStateService', productsController]);

function productsController($scope, productsService, $routeParams, $location, queryBuilderService, toastr, sortOptionsService, $rootScope, productsStateService) {
    $scope.productName = $routeParams.productName;
    $scope.cityId = $routeParams.city;
    $scope.skip = 0;
    $scope.query = "all";
    $scope.itemsPerPage = 6;
    $scope.sortOptions = sortOptionsService.getSortOptions();
    $scope.sortBy = $scope.sortOptions[2];
    $scope.count = 0;
    $scope.currentPage = 0;
    $scope.totalPages = 0;
    $scope.loading = false;

    $scope.products = [];

    $scope.init = function () {

        if (($rootScope.fromUrl.indexOf('/product/') > -1) && (productsStateService.hasState() === true))
            $scope.getLastState();
        else {
            $scope.setInitialCriteria();
            $scope.getProducts();
        }
    };

    $scope.getLastState = function() {
        let state = productsStateService.get();

        $scope.skip = state.skip;
        $scope.query = state.query;
        $scope.itemsPerPage = state.itemsPerPage;
        $scope.sortBy = $scope.sortOptions[state.sortByIndex];
        $scope.count = state.count;
        $scope.currentPage = state.currentPage;
        $scope.totalPages = state.totalPages;
        $scope.products = state.products;
    }

    $scope.setState = function() {
        let state = {};

        state.skip = $scope.skip;
        state.query = $scope.query;
        state.itemsPerPage = $scope.itemsPerPage;
        state.sortByIndex = $scope.sortOptions.indexOf($scope.sortBy);
        state.count = $scope.count;
        state.currentPage = $scope.currentPage;
        state.totalPages = $scope.totalPages;
        state.products = $scope.products;

        productsStateService.set(state);
    }

    $scope.setInitialCriteria = function () {
        let queryParts = new Array();
        if ($scope.productName !== "allProducts") {
            let queryPartTitle = new QueryBuilderPart('title', $scope.productName, $scope.productName, true);
            queryParts.push(queryPartTitle);
        }

        if ($scope.cityId !== "allCities") {
            let queryPartCity = new QueryBuilderPart('cityId', $scope.cityId, $scope.cityId, null);
            queryParts.push(queryPartCity);
        }

        if (queryParts.length > 0)
            $scope.query = queryBuilderService.buildQuery(queryParts); 
    }

    $scope.getProducts = function () {
        $scope.loading = true;

        $scope.itemsPerPage = $scope.itemsPerPage ? $scope.itemsPerPage : 4;

        productsService.queryProducts($scope.query,
                $scope.itemsPerPage,
                $scope.skip,
                $scope.sortBy.value,
                $scope.sortBy.sortDirection)
            .then(function(productsAndCount) {
                    if (productsAndCount.data) {
                        $scope.products = productsAndCount.data.products;
                        $scope.count = productsAndCount.data.count;
                        $scope.calculatePages();
                    }
                },
                function(error) {
                    toastr.error('An error occured!');
                    $scope.doSomethingWithError(error);
                })
            .finally(function() {
                $scope.loading = false;
            });
    }

    $scope.resetGetProducts = function () {
        $scope.skip = 0;
        $scope.getProducts();
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

    $scope.showProduct = function (id) {
        $scope.setState();
        $location.url('/product/' + id);
    };

    $scope.doSomethingWithError = function (error) {
        console.log(error);
    }

    $scope.init();
}