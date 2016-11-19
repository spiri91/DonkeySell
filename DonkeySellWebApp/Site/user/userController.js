app.controller('userController', ['$scope', 'usersService', 'productsService', '$location', '$routeParams', 'toastr', '$mdDialog','favoritesService', '$uibModal', userController]);

function userController($scope, usersService, productsService, $location, $routeParams, toastr, $mdDialog, favoritesService, $uibModal) {
    $scope.username = $routeParams.username;
    $scope.showEditButtons = $scope.$parent.username === $scope.username;
    $scope.user = {};
    $scope.products = [];

    $scope.init = function () {
        usersService.getUser($scope.username)
            .then(function (user) {
                if(user.data)
                    $scope.user = user.data;
            }, function(error) {
                $scope.doSomethingWithError(error);
            });

        productsService.getProductsOfUser($scope.username)
            .then(function (products) {
                if(products.data)
                    $scope.products = products.data;
            }, function(error) {
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

    $scope.doSomethingWithError = function(error) {
        console.log(error);
    }

    $scope.init();
}