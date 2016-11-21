'use strict';

app.controller('mainController',
[
    '$scope', '$uibModal', 'usersService', '$location', 'toastr', 'favoritesService', 'othersService',
    'storageService', mainController
]);

function mainController($scope, $uibModal, usersService, $location, toastr, favoritesService, othersService, storageService) {
    $scope.user = {};
    $scope.username = "";
    $scope.favorites = [];
    $scope.unreadMessages = [];
    $scope.token = '';

    $scope.cities = [];
    $scope.categories = [];

    $scope.login = function () {
        let modalInstance = $uibModal.open({
            templateUrl: 'Site/login/login.html',
            controller: 'loginController',
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
        } else {
            
        }
    }

    $scope.showErrorMessage = function (message) {
        toastr.error(message);
    }

    $scope.getUser = function () {
        return usersService.getUser($scope.username)
           .then(function (result) {
               if (result.data)
                   $scope.user = result.data;
           }, function (error) {
               $scope.showErrorMessage("Please logIn again!");
                $scope.doSomethingWithError(error);
            });
    }

    $scope.getFavorites = function () {
        favoritesService.getFavoritesProducts($scope.username, $scope.token)
            .then(function (products) {
                if (products.data)
                    $scope.favorites = products.data;
            }, function (error) {
               $scope.doSomethingWithError(error);
            });
    };

    $scope.getUnreadMessages = function () {
        usersService.getUnreadMessages($scope.username, $scope.token)
            .then(function (messages) {
                if(messages.data)
                    $scope.unreadMessages = messages.data;
            }, function(error) {
               $scope.doSomethingWithError(error);
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
              if(result.data)
                $scope.cities = result.data;
          }, function(error) {
               $scope.doSomethingWithError(error);
            });
    }

    $scope.getCategories = function () {
        othersService.getCategories()
         .then(function (result) {
             if(result.data)
                $scope.categories = result.data;
         }, function(error) {
               $scope.doSomethingWithError(error);
            });
    };

    $scope.removeProductFromFavorites = function (product) {
        favoritesService.deleteFromFavoritesProducts($scope.username, $scope.token, product.id)
            .then(function () {
                toastr.success("Product removed!");
                let index = $scope.favorites.indexOf(product);
                $scope.favorites.splice(index, 1);
            }, function(error) {
               $scope.doSomethingWithError(error);
            });
    };

    $scope.markMessageRead = function (message) {
        usersService.markMessageRead($scope.user.userName, $scope.token, message.id)
            .then(function () {
                let index = $scope.unreadMessages.indexOf(message);
                $scope.unreadMessages.splice(index, 1);
            }, function() {
                $scope.doSomethingWithError(error);
            });

        $location.url('/product/' + message.productId);
    };

    $scope.goToProduct = function (id) {
        $location.url('/product/' + id);
    }

    $scope.doSomethingWithError = function(error) {
        console.log(error);
    }

    $scope.logout = function () {
        usersService.signOut();
        $scope.user = {};
        $scope.username = $scope.token = '';
        $scope.favorites = $scope.unreadMessages = [];
        $scope.$broadcast('logout');
        toastr.success('You logged out!');
    };

    $scope.getCitiesAndCategories();
    $scope.init();
};