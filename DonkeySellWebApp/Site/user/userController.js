app.controller('userController', ['$scope', 'usersService', 'productsService', '$location', '$routeParams', 'cookiesService', 'toastr', '$mdDialog','favoritesService', userController]);

function userController($scope, usersService, productsService, $location, $routeParams, cookiesService, toastr, $mdDialog, favoritesService) {
    $scope.username = $routeParams.username;
    $scope.user = {};
    $scope.products = [];

    $scope.init = function () {
        usersService.getUser($scope.username)
            .then(function(user) {
                $scope.user = user.data;
            });
        productsService.getProductsOfUser($scope.username)
            .then(function (products) {
                $scope.products = products.data;
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
                        cookiesService.deleteCookie('usernameDonkeySell');
                        $scope.$parent.username = '';
                        usersService.signOut();
                        $location.url('/home');
                        toastr.success('User deleted!');
                    });
            });
    };

    $scope.deleteProduct = function (id) {
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
                    });
            });
    };

    $scope.getProduct = function (id) {
        $location.url('/product/' + id);
    };

    $scope.addToFavorites = function (product) {
        if (!$scope.$parent.token) {
            toastr.error('Please login first!');
            return;
        }

        if ($scope.$parent.favorites.indexOf(product) >= 0) {
            toastr.error('This product is allready added!');
            return;
        }

        favoritesService.postProductToFavorites($scope.$parent.user.userName, $scope.$parent.token, product.id)
            .then(function () {
                $scope.$parent.favorites.push(product);
                toastr.success('Product added to Favorites!');
            });
    };

    $scope.init();
}