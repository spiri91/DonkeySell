app.controller('advanceSearchController', ['$scope', 'productsService', '$location', 'othersService', 'queryBuilderService', advanceSearchController]);

function advanceSearchController($scope, productsService, $location, othersService, queryBuilderService) {

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

    $scope.itemsPerPage = 5;
    $scope.skip = 0;
    $scope.query = "";
    $scope.sortBy = "DatePublished";
    $scope.sortOptions = ["Price", "DatePublished", "UserName"];


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
        $scope.buidQuery();
        $scope.itemsPerPage = $scope.itemsPerPage ? $scope.itemsPerPage : 4;
        productsService.queryProducts($scope.query, $scope.itemsPerPage, $scope.skip, $scope.sortBy)
            .then(function (products) {
                if (products.data) {
                    $scope.products = products.data;
                    $scope.checkForEndOfTheList();
                }
            }, function (error) {
                $scope.endOfList = true;
                $scope.doSomethingWithError(error);
            });
    };

    $scope.resetSkipAndGetProducts = function () {
        $scope.skip = 0;
        $scope.getProducts();
    }

    $scope.checkForEndOfTheList = function () {
        if ($scope.products.length < $scope.itemsPerPage)
            $scope.endOfList = true;
        else
            $scope.endOfList = false;
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
    };

    $scope.setDates = function () {
        $scope.minDate.setDate($scope.minDate.getDate() - 31);
        $scope.maxDate.setDate($scope.maxDate.getDate() + 1);
    };

    $scope.getProduct = function (id) {
        $location.url('/product/' + id);
    }

    $scope.doSomethingWithError = function (error) {
        console.log(error);
    }

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