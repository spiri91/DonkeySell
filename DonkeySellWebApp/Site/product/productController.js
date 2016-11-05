app.controller('productController', ['$scope', 'messagesService', 'productsService', 'usersService', '$routeParams', 'toastr', '$mdDialog', 'favoritesService', '$mdBottomSheet', productController]);

function productController($scope, messagesService, productsService, usersService, $routeParams, toastr, $mdDialog, favoritesService, $mdBottomSheet) {
    $scope.id = $routeParams.productId;
    $scope.product = {};
    $scope.productOwner = {};
    $scope.messages = [];
    $scope.newMessage = "";
    $scope.token = "";
    $scope.selectedImage = "";

    var originatorEv;

    $scope.init = function () {
        $scope.getProductAndProductOwner();
        $scope.getMessages();


    }

    $scope.getProductOwner = function () {
        usersService.getUser($scope.product.userName)
            .then(function (user) {
                $scope.productOwner = user.data;
            });
    }

    $scope.showGridBottomSheet = function () {
        $scope.alert = '';
        $mdBottomSheet.show({
            templateUrl: 'Site/product/contactPage/contactPage.html',
            controller: 'contactPageController',
            locals: {
                Item: {
                    'phone': $scope.productOwner.phone,
                    'facebook': $scope.productOwner.facebook,
                    'email': $scope.productOwner.email,
                    'twitter': $scope.productOwner.twitter
                }
            },
            clickOutsideToClose: true
        });
    };

    $scope.getProductAndProductOwner = function () {
        productsService.getProduct(this.id)
            .then(function (product) {
                $scope.product = product.data;
                $scope.selectedImage = $scope.product.images[0] ? $scope.product.images[0].value : "";
            })
            .then(function () {
                usersService.getUser($scope.product.userName)
                    .then(function (user) {
                        $scope.productOwner = user.data;
                    });
            });
    }

    $scope.getMessages = function () {
        messagesService.getMessagesForProduct(this.id)
            .then(function (messages) {
                $scope.messages = messages.data;
            });
    }

    $scope.openMenu = function ($mdOpenMenu, ev) {
        originatorEv = ev;
        $mdOpenMenu(ev);
    };

    $scope.addProductToFavorites = function () {
        if (!$scope.$parent.token) {
            toastr.error('Please login first!');
            return;
        }

        if ($scope.$parent.favorites.indexOf($scope.product) >= 0) {
            toastr.error('This product is allready added!');
            return;
        }

        favoritesService.postProductToFavorites($scope.$parent.user.userName, $scope.$parent.token, $scope.product.id)
            .then(function () {
                $scope.$parent.favorites.push($scope.product);
                toastr.success('Product added to Favorites!');
            });
    };

    $scope.saveMessage = function () {
        if ($scope.$parent.token) {
            let message = new Message(null, $scope.$parent.user.userName, $scope.newMessage, Date.now(), $scope.id, false);
            messagesService.postMessageForProduct(message, $scope.$parent.token)
                .then(function (newMessage) {
                    $scope.messages.push(newMessage.data);
                    $scope.newMessage = '';
                });
        } else {
            toastr.error("Please login first!");
        }
    }

    $scope.deleteMessage = function (id) {
        if ($scope.getToken()) {
            messagesService.deleteMessage($scope.id, id, $scope.token)
                .then(function (id) {
                    let message = $.grep($scope.messages, function (m) { return m.id === id.data });
                    let index = $scope.messages.indexOf(message);
                    $scope.messages.splice(index, 1);
                });
        }
    }

    $scope.getToken = function () {
        $scope.token = usersService.getToken();
        if (!$scope.token) {
            toastr.error('Please login first!');
            return false;
        }

        return true;
    };

    $scope.setSelected = function(index) {
        $scope.selectedImage = $scope.product.images[index].value;
    };

    $scope.init();
}