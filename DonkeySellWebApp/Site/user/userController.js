app.controller('userController', ['$scope', 'usersService', 'productsService', '$location',
    '$routeParams', 'toastr', '$mdDialog', 'favoritesService', '$uibModal','$rootScope', 'userStateService', userController]);

function userController($scope, usersService, productsService, $location, $routeParams, toastr, $mdDialog, favoritesService, $uibModal, $rootScope, userStateService) {
    $scope.username = $routeParams.username;
    $scope.showEditButtons = $scope.$parent.username === $scope.username;
    $scope.user = {};
    $scope.products = [];

    $scope.init = function () {
        if (userStateService.hasState() === true && (($rootScope.fromUrl.indexOf('/product/') > -1) || ($rootScope.fromUrl.indexOf('/updateCreateProduct/') > -1)) && userStateService.getUsernameFromState() === $scope.username)
            $scope.getLastState();
        else {
            $scope.getUser();
            $scope.getUsersProducts();
        }
    };

    $scope.getLastState = function() {
        let state = userStateService.get();

        $scope.username = state.username;
        $scope.showEditButtons = state.showEditButtons;
        $scope.user = state.user;
        $scope.products = state.products;
    };

    $scope.setState = function() {
        let state = {};
        state.username = $scope.username;
        state.showEditButtons = $scope.showEditButtons;
        state.user = $scope.user;
        state.products = $scope.products;

        userStateService.set(state);
    }

    $scope.getUsersProducts = function() {
        productsService.getProductsOfUser($scope.username)
           .then(function (products) {
               if (products.data)
                   $scope.products = products.data;
           }, function (error) {
               $scope.doSomethingWithError(error);
           });
    }

    $scope.getUser = function() {
        usersService.getUser($scope.username)
            .then(function (user) {
                if (user.data)
                    $scope.user = user.data;
            }, function (error) {
                $scope.doSomethingWithError(error);
            });
    };

    $scope.deleteUser = function () {
        var confirm = $mdDialog.confirm()
         .title('Are you sure you want to leave?! :(')
         .ok("Yes, i'm sure!")
         .cancel('No!');

        $mdDialog.show(confirm)
            .then(function() {
                usersService.deleteUser($scope.username, usersService.getToken())
                    .then(function () {
                        $scope.$parent.logout();
                        $location.url('/home');
                        toastr.success('User deleted!');
                    }, function(error) {
                        $scope.doSomethingWithError(error);
                    });
            });
    };

    $scope.deleteProduct = function (id, $event) {
        $event.stopPropagation();
        var confirm = $mdDialog.confirm()
          .title('Would you like to delete this product?')
          .ok("Yes, i'm sure!")
          .cancel('No!');

        $mdDialog.show(confirm)
            .then(function() {
                productsService.deleteProduct(id, usersService.getToken())
                    .then(function () {
                        let product = $scope.products.filter(function (x) { return x.id === id; })[0];
                        let index = $scope.products.indexOf(product);
                        $scope.products.splice(index, 1);
                    }, function(error) {
                        $scope.doSomethingWithError(error);
                    });
            });
    };

    $scope.getProduct = function (id) {
        $scope.setState();
        $location.url('/product/' + id);
    };

    $scope.addToFavorites = function (product, $event) {
        $event.stopPropagation();
        if (!$scope.$parent.token) {
            toastr.error('Please login first!');
            return;
        }
        
        if ($scope.productAllreadyInFav(product.id)) {
            toastr.error('This product is allready added!');
            return;
        }

        favoritesService.postProductToFavorites($scope.$parent.user.userName, $scope.$parent.token, product.id)
            .then(function () {
                $scope.$parent.favorites.push(product);
                toastr.success('Product added to Favorites!');
            }, $scope.doSomethingWithError(error));
    };

    $scope.productAllreadyInFav = function(id) {
        let prod = $scope.$parent.favorites.filter(function (x) { return x.id === id });
        if (prod.length > 0)
            return true;

        return false;
    }

    $scope.changePassword = function() {
        var modalInstance = $uibModal.open({
                animation:true,
                templateUrl: 'Site/changePassword/changePassword.html',
                controller: 'changePasswordController',
                resolve: {
                    username: function (){return $scope.username}
                }
            });

            modalInstance.result.then(function () {
                $scope.init();
            });
    };

    $scope.editProduct = function(productId, $event) {
        $event.stopPropagation();
        $scope.setState();
        $location.url('/updateCreateProduct/' + productId);
    };

    $scope.doSomethingWithError = function(error) {
        console.log(error);
    }

    $scope.init();
}