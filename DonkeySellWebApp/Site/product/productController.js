app.controller('productController', ['$scope', 'messagesService', 'productsService', 'usersService', '$routeParams', 'toastr', '$mdDialog', 'favoritesService', '$mdBottomSheet','$uibModal', productController]);

function productController($scope, messagesService, productsService, usersService, $routeParams, toastr, $mdDialog, favoritesService, $mdBottomSheet, $uibModal) {
    $scope.id = $routeParams.productId;
    $scope.product = {};
    $scope.productOwner = {};
    $scope.messages = [];
    $scope.newMessage = "";
    $scope.selectedImage = "";

    var originatorEv;

    $scope.init = function () {
        $scope.getProductAndProductOwner();
        $scope.getMessages();
    }

    $scope.getProductOwner = function () {
        usersService.getUser($scope.product.userName)
            .then(function (user) {
                if(user.data)
                    $scope.productOwner = user.data;
            }, function(error) {
                $scope.doSomethingWithError(error)
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
                if (product.data) {
                    $scope.product = product.data;
                    $scope.selectedImage = $scope.product.images[0] ? $scope.product.images[0].value : "";
                }
            }, function(error) { $scope.doSomethingWithError(error); })
            .then(function () {
                $scope.getProductOwner();
            });
    }

    $scope.getMessages = function () {
        messagesService.getMessagesForProduct(this.id)
            .then(function (messages) {
                if(messages.data)
                    $scope.messages = messages.data;
            }, function(error) {
                $scope.doSomethingWithError(error);
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
            }, function(error) {
                $scope.doSomethingWithError(error);
            });
    };

    $scope.saveMessage = function () {
        if (!$scope.newMessage)
            return;

        if ($scope.$parent.token ) {
            let message = new Message(null, $scope.$parent.user.userName, $scope.newMessage, Date.now(), $scope.id, false);
            messagesService.postMessageForProduct(message, $scope.$parent.token)
                .then(function (newMessage) {
                    if(newMessage.data)
                        $scope.messages.push(newMessage.data);
                    $scope.newMessage = '';
                }, function(error) { $scope.doSomethingWithError(error); });
        } else {
            toastr.error("Please login first!");
        }
    }

    $scope.deleteMessage = function (id) {
        if ($scope.$parent.token) {
            messagesService.deleteMessage($scope.id, id, $scope.$parent.token)
                .then(function (id) {
                    let message = $.grep($scope.messages, function (m) { return m.id === id.data });
                    let index = $scope.messages.indexOf(message);
                    $scope.messages.splice(index, 1);
                }, function(error){$scope.doSomethingWithError(error)});
        }
    }

    $scope.openImage = function(index) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'Site/gallery/gallery.html',
            controller: 'galleryController',
            size:'lg',
            resolve: {
                images: function () { return $scope.product.images },
                index: function () { return index }
            }
        });

        modalInstance.result.then(function () {
            $scope.init();
        });
    }

    $scope.setSelected = function(index) {
        $scope.selectedImage = $scope.product.images[index].value;
    };

    $scope.doSomethingWithError = function(error) {
        console.log(error);
    }

    $scope.init();
}