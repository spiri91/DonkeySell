app.controller('mainController',
[
    '$scope', '$uibModal', 'usersService', '$location', 'toastr', 'favoritesService', 'othersService',
    'storageService', '$timeout', '$mdSidenav', '$log', '$mdDialog', '$rootScope','$q', mainController
]);

function mainController($scope, $uibModal, usersService, $location, toastr,
    favoritesService, othersService, storageService, $timeout, $mdSidenav, $log, $mdDialog, $rootScope, $q) {
    $scope.user = {};
    $scope.username = "";
    $scope.favorites = [];
    $scope.unreadMessages = [];
    $scope.token = '';
    $scope.productPopups = [];

    $scope.cities = [];
    $scope.categories = [];

    $scope.buildToggler = function (navId) {
        return function () {
            $mdSidenav(navId)
                .toggle();
        }
    }

    $scope.toggleLeft = $scope.buildToggler('left');
    $scope.isOpenLeft = function () {
        return $mdSidenav('left').isOpen();
    };

    $scope.login = function () {
        let modalInstance = $uibModal.open({
            templateUrl: 'Site/login/login.html',
            controller: 'loginController'
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

    $scope.showErrorMessage = function (message) {
        toastr.error(message);
    }

    $scope.getUser = function () {
        usersService.getUser($scope.username)
          .then(function (result) {
              if (result.data)
                  $scope.user = result.data;
          }, function (error) {
              $scope.showErrorMessage("Please login again!");
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
                if (messages.data)
                    $scope.unreadMessages = messages.data;
            }, function (error) {
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
        $q.all([$scope.getCities(), $scope.getCategories])
            .then(() => {
                $rootScope.$broadcast('citiesAndCategoriesLoaded');});
    }

    $scope.getCities = function () {
        return othersService.getCities()
          .then(function (result) {
              if (result.data)
                  $scope.cities = result.data;
          }, function (error) {
              $scope.doSomethingWithError(error);
          });
    }

    $scope.getCategories = function () {
       return othersService.getCategories()
         .then(function (result) {
             if (result.data)
                 $scope.categories = result.data;
         }, function (error) {
             $scope.doSomethingWithError(error);
         });
    };

    $scope.removeProductFromFavorites = function (product) {
        favoritesService.deleteFromFavoritesProducts($scope.username, $scope.token, product.id)
            .then(function () {
                toastr.success("Product removed!");
                let index = $scope.favorites.indexOf(product);
                $scope.favorites.splice(index, 1);
            }, function (error) {
                $scope.doSomethingWithError(error);
            });
    };

    $scope.markMessageRead = function (message) {
        usersService.markMessageRead($scope.user.userName, $scope.token, message.id)
            .then(function () {
                let index = $scope.unreadMessages.indexOf(message);
                $scope.unreadMessages.splice(index, 1);
            }, function () {
                $scope.doSomethingWithError(error);
            });

        $location.url('/product/' + message.productId);
    };

    $scope.goToProduct = function (id) {
        $location.url('/product/' + id);
    }

    $scope.doSomethingWithError = function (error) {
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

    $scope.contactMe = function () {
        $mdDialog.show(
            $mdDialog.alert()
            .clickOutsideToClose(true)
            .title('Contact me')
            .textContent('Send me a mail at:  spataru.ionut91@gmail.com')
            .ok('Got it!')
        );
    }

    $rootScope.$on('$locationChangeSuccess', function (e, newLocation, oldLocation) {
        $rootScope.fromUrl = oldLocation;
    });

    $scope.getCitiesAndCategories();
    $scope.init();
};