﻿var app = angular.module('app', ['ngRoute', 'toastr', 'ui.bootstrap', 'ngMaterial', 'toggle-switch', 'flow', 'ngAnimate', 'ngMessages']);

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
         .when('/about',
        {
            templateUrl: 'Site/about/about.html'
        })
        .otherwise({ redirectTo: '/' });

    $mdThemingProvider.theme('dark-blue').backgroundPalette('blue').dark();
    $mdThemingProvider.theme('dark-purple').backgroundPalette('deep-purple').dark();

    angular.extend(toastrConfig, {
        positionClass: 'toast-top-right'
    });

    $mdAriaProvider.disableWarnings();
});

app.constant('apiRootAddress', "http://spiri91-001-site1.ftempurl.com/api/"); //"http://localhost:57792/api/" ,  "http://spiri91-001-site1.ftempurl.com/api/"