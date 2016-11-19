var app = angular.module('app', ['ngRoute', 'toastr', 'ui.bootstrap', 'ODataResources', 'ngCookies', 'ngMaterial', 'toggle-switch', 'flow']);

app.config(function ($routeProvider, $mdThemingProvider, toastrConfig, $mdAriaProvider) {
    $routeProvider
        .when('/',
        {
            templateUrl: "Site/home/home.html",
            controller: 'homeController'
        })
        .when('/home',
        {
            templateUrl: "Site/home/home.html",
            controller: 'homeController'
        })
        .when('/products/:productName/:city',
        {
            templateUrl: 'Site/products/products.html',
            controller: 'productsController'
        })
        .when('/product/:productId',
        {
            templateUrl: 'Site/product/product.html',
            controller: 'productController'
        })
        .when('/register',
        {
            templateUrl: 'Site/editRegister/editRegister.html',
            controller: 'editRegisterController'
        })
        .when('/edit/:username',
        {
            templateUrl: 'Site/editRegister/editRegister.html',
            controller: 'editRegisterController'
        })
        .when('/updateCreateProduct/:productId',
        {
            templateUrl: 'Site/updateCreateProduct/updateCreateProduct.html',
            controller: 'updateCreateProductController'
        })
         .when('/user/:username',
        {
            templateUrl: 'Site/user/user.html',
            controller: 'userController'
        })
         .when('/advanceSearch',
        {
            templateUrl: 'Site/advanceSearch/advanceSearch.html',
            controller: 'advanceSearchController'
        })
        .otherwise({ redirectTo: '/' });

    $mdThemingProvider.theme('dark-blue').backgroundPalette('blue').dark();
    $mdThemingProvider.theme('dark-purple').backgroundPalette('deep-purple').dark();

    angular.extend(toastrConfig, {
        positionClass: 'toast-top-right'
    });

    $mdAriaProvider.disableWarnings();
});

app.constant('apiRootAddress', "http://localhost:57792/api/");