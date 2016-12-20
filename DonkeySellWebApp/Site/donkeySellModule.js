var app = angular.module('app', ['ngRoute', 'toastr', 'ui.bootstrap', 'ngMaterial', 'toggle-switch', 'flow', 'ngAnimate', 'ngMessages', 'uiGmapgoogle-maps']);

app.config(['$routeProvider', '$mdThemingProvider', 'toastrConfig', '$mdAriaProvider', 'uiGmapGoogleMapApiProvider', function ($routeProvider, $mdThemingProvider, toastrConfig, $mdAriaProvider, uiGmapGoogleMapApiProvider) {
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
          .when('/customAlerts',
        {
            templateUrl: 'Site/customAlerts/customAlerts.html',
            controller: 'customAlertsController'
        })
         .when('/about',
        {
            templateUrl: 'Site/about/about.html'
        })
        .otherwise({ redirectTo: '/' });

    $mdThemingProvider.theme('dark-blue').backgroundPalette('blue').dark();
    $mdThemingProvider.theme('dark-purple').backgroundPalette('deep-purple').dark();

    uiGmapGoogleMapApiProvider.configure({
        key: 'AIzaSyDw1ic0x3wkaLO_1B79VS28jaoHCdyQ7Qo',
        v: '3.26',
        libraries: 'weather,geometry,visualization'
    });

    angular.extend(toastrConfig, {
        positionClass: 'toast-top-right'
    });

    $mdAriaProvider.disableWarnings();
}]);

app.constant('apiRootAddress', "http://localhost:57792/api/"); //"http://localhost:57792/api/" ,  "http://spiri91-001-site1.ftempurl.com/api/"