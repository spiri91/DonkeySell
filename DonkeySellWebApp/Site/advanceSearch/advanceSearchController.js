app.controller('advanceSearchController', ['$scope', 'productsService', '$location',
    'othersService', 'queryBuilderService', 'sortOptionsService', '$rootScope', 'advanceSearchStateService', 'productNavigationService', advanceSearchController]);

function advanceSearchController($scope, productsService, $location, othersService,
    queryBuilderService, sortOptionsService, $rootScope, advanceSearchStateService, productNavigationService) {

    $scope.products = [];
    $scope.cities = [];
    $scope.categories = [];
    $scope.selectedCategoryId = {};
    $scope.selectedCityId = {};
    $scope.minPrice = 100;
    $scope.maxPrice = 500;
    $scope.minDate = new Date();
    $scope.maxDate = new Date();
    $scope.searchInDescriptionAlso = false;
    $scope.title = "";
    $scope.endOfList = false;
    $scope.count = 0;
    $scope.sortOptions = sortOptionsService.getSortOptions();
    $scope.sortBy = $scope.sortOptions[2];
    $scope.loading = false;

    $scope.itemsPerPage = 5;
    $scope.skip = 0;
    $scope.query = "";
    $scope.currentPage = 0;
    $scope.totalPages = 0;

    $scope.buidQuery = function () {
        let queryParts = new Array();
        let queryPartPrice = new QueryBuilderPart('price', $scope.minPrice, $scope.maxPrice);
        queryParts.push(queryPartPrice);

        if ($scope.minDate && $scope.maxDate) {
            let minDate = getStringFromDate($scope.minDate);
            let maxDate = getStringFromDate($scope.maxDate);
            let queryPartDatePosted = new QueryBuilderPart('datePublished', minDate, maxDate);
            queryParts.push(queryPartDatePosted);
        }

        if (!isNaN($scope.selectedCityId)) {
            let queryPartCity = new QueryBuilderPart('cityId', $scope.selectedCityId, $scope.selectedCityId);
            queryParts.push(queryPartCity);
        }

        if (!isNaN($scope.selectedCategoryId)) {
            let queryPartCategory = new QueryBuilderPart('categoryId',
                $scope.selectedCategoryId,
                $scope.selectedCategoryId);
            queryParts.push(queryPartCategory);
        }

        if ($scope.title) {
            let queryPartTitle = new QueryBuilderPart('title',
                $scope.title,
                $scope.title,
                true,
                $scope.searchInDescriptionAlso);
            queryParts.push(queryPartTitle);
        }

        if ($scope.searchInDescriptionAlso === true && $scope.title) {
            let queryPartDescription = new QueryBuilderPart('description', $scope.title, $scope.title, true);
            queryParts.push(queryPartDescription);
        }

        $scope.query = queryBuilderService.buildQuery(queryParts);
    }

    $scope.setState = function () {
        let state = {};

        state.products = $scope.products;
        state.selectedCategoryId = $scope.selectedCategoryId;
        state.selectedCityId = $scope.selectedCityId;
        state.minPrice = $scope.minPrice;
        state.maxPrice = $scope.maxPrice;
        state.minDate = $scope.minDate;
        state.maxDate = $scope.maxDate;
        state.searchInDescriptionAlso = $scope.searchInDescriptionAlso;
        state.title = $scope.title;
        state.endOfList = $scope.endOfList;
        state.count = $scope.count;
        state.sortByIndex = $scope.sortOptions.indexOf($scope.sortBy);
        state.itemsPerPage = $scope.itemsPerPage;
        state.skip = $scope.skip;
        state.query = $scope.query;
        state.currentPage = $scope.currentPage;
        state.totalPages = $scope.totalPages;

        advanceSearchStateService.set(state);
    };

    $scope.getLastState = function () {
        let state = advanceSearchStateService.get();

        $scope.products = state.products;
        $scope.selectedCategoryId = state.selectedCategoryId;
        $scope.selectedCityId = state.selectedCityId;
        $scope.minPrice = state.minPrice;
        $scope.maxPrice = state.maxPrice;
        $scope.minDate = state.minDate;
        $scope.maxDate = state.maxDate;
        $scope.searchInDescriptionAlso = state.searchInDescriptionAlso;
        $scope.title = state.title;
        $scope.endOfList = state.endOfList;
        $scope.count = state.count;
        $scope.sortBy = $scope.sortOptions[state.sortByIndex];
        $scope.itemsPerPage = state.itemsPerPage;
        $scope.skip = state.skip;
        $scope.query = state.query;
        $scope.currentPage = state.currentPage;
        $scope.totalPages = state.totalPages;
    };

    $scope.getCities = function () {
        $scope.cities = $scope.$parent.cities;
    }

    $scope.getCategories = function () {
        $scope.categories = $scope.$parent.categories;
    }

    $scope.firstSearch = function () {
        $scope.skip = 0;
        $scope.getProducts();
    }

    $scope.getProducts = function () {
        $scope.loading = true;
        $scope.buidQuery();
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
                    $scope.endOfList = true;
                    $scope.doSomethingWithError(error);
                })
            .finally(function() {
                $scope.loading = false;
            });
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

    $scope.resetSkipAndGetProducts = function () {
        $scope.skip = 0;
        $scope.getProducts();
    }

    $scope.showNextProducts = function () {
        $scope.skip += $scope.itemsPerPage;
        $scope.getProducts();
    }

    $scope.showPreviousProducts = function () {
        if ($scope.skip !== 0) {
            $scope.skip -= $scope.itemsPerPage;
            $scope.getProducts();
        }
    }

    $scope.init = function () {
        $scope.getCategories();
        $scope.getCities();
        $scope.setDates();

        if (($rootScope.fromUrl.indexOf('/product/') > -1) && (advanceSearchStateService.hasState()===true))
            $scope.getLastState();
    };

    $scope.setDates = function () {
        $scope.minDate.setDate($scope.minDate.getDate() - 31);
        $scope.maxDate.setDate($scope.maxDate.getDate() + 1);
    };

    $scope.setProductNavigation = function () {
        let productsIds = $scope.products.map(function (product) { return product.id; });
        productNavigationService.setValues($scope.query, $scope.skip, $scope.sortBy, $scope.count, productsIds);
    }

    $scope.getProduct = function (id) {
        $scope.setProductNavigation();
        $location.url('/product/' + id);
    }

    $scope.doSomethingWithError = function (error) {
        console.log(error);
    }

    $scope.showProduct = function (id) {
        $scope.setState();
        $location.url('/product/' + id);
    };

    $scope.showProduct = function (id) {
        $scope.setState();
        $location.url('/product/' + id);
    };

    $scope.init();

    function getStringFromDate(date) {
        let year = date.getFullYear();
        let month = (1 + date.getMonth()).toString();
        month = month.length > 1 ? month : '0' + month;
        let day = date.getDate().toString();
        day = day.length > 1 ? day : '0' + day;
        return month + '.' + day + '.' + year;
    }
}