'use strict';

app.controller('mainController', ['$scope', '$uibModal', 'cookiesService', 'usersService', '$location', 'toastr', 'favoritesService', 'othersService', 'storageService', mainController]);

function mainController($scope, $uibModal, cookiesService, usersService, $location, toastr, favoritesService, othersService, storageService) {
    $scope.user = {};
    $scope.username = "";
    $scope.favorites = [];
    $scope.unreadMessages = [];
    $scope.token = '';

    $scope.isOpen = false;
    $scope.cities = [];
    $scope.categories = [];

    $scope.login = function () {
        var modalInstance = $uibModal.open({
            templateUrl: 'Site/login/login.html',
            controller: 'loginController',
            size: 'sm'
    });

        modalInstance.result.then(function () {
            $scope.init();
        });
    };

    $scope.init = function () {
        $scope.token = usersService.getToken();
        $scope.username = storageService.get("username");
        if ($scope.token && $scope.username) {
            $scope.getUser();
            $scope.getFavorites();
            $scope.getUnreadMessages();
        }
    }

    $scope.getUser = function () {
        return usersService.getUser($scope.username)
           .then(function (result) {
               $scope.user = result.data;
           });
    }

    $scope.getFavorites = function () {
        favoritesService.getFavoritesProducts($scope.username, $scope.token)
            .then(function (products) {
                $scope.favorites = products.data;
            });
    };

    $scope.getUnreadMessages = function () {
        usersService.getUnreadMessages($scope.username, $scope.token)
            .then(function (messages) {
                $scope.unreadMessages = messages.data;
            });
    };

    $scope.postNewProduct = function () {
        if ($scope.token)
            $location.url('/updateCreateProduct/0');
        else {
            $scope.askForLogin();
        }
    };

    $scope.showUser = function () {
        if ($scope.token)
            $location.url('/user/' + $scope.username);
        else {
            $scope.askForLogin();
        }
    }

    $scope.askForLogin = function () {
        toastr.error('Please login first!');
    }

    $scope.getCitiesAndCategories = function () {
        $scope.getCities();
        $scope.getCategories();
    }

    $scope.getCities = function () {
        othersService.getCities()
          .then(function (result) {
              $scope.cities = result.data;
          });
    }

    $scope.getCategories = function () {
        othersService.getCategories()
         .then(function (result) {
             $scope.categories = result.data;
         });
    };

    $scope.removeProductFromFavorites = function (product) {
        favoritesService.deleteFromFavoritesProducts($scope.username, $scope.token, product.id)
            .then(function () {
                toastr.success("Product removed!");
                let index = $scope.favorites.indexOf(product);
                $scope.favorites.splice(index, 1);
            });
    };

    $scope.markMessageRead = function (message) {
        usersService.markMessageRead($scope.user.userName, $scope.token, message.id)
            .then(function () {
                let index = $scope.unreadMessages.indexOf(message);
                $scope.unreadMessages.splice(index, 1);
            });

        $location.url('/product/' + message.productId);
    };

    $scope.goToProduct = function (id) {
        $location.url('/product/' + id);
    }

    $scope.logout = function() {
        usersService.signOut();
        $scope.user = {};
        $scope.username = "";
        $scope.favorites = [];
        $scope.unreadMessages = [];
        $scope.token = '';
        toastr.success('You logged out!');
    };

    $scope.getCitiesAndCategories();
    $scope.init();
};