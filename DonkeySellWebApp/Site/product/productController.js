app.controller('productController', ['$scope', 'messagesService', 'productsService', 'usersService', '$routeParams', 'toastr',
    '$mdDialog', 'favoritesService', '$mdBottomSheet', '$uibModal', 'productNavigationService', '$location', productController]);

function productController($scope, messagesService, productsService, usersService, $routeParams, toastr,
    $mdDialog, favoritesService, $mdBottomSheet, $uibModal, productNavigationService, $location) {
    $scope.id = $routeParams.productId;
    $scope.product = {};
    $scope.productOwner = {};
    $scope.messages = [];
    $scope.newMessage = "";
    $scope.selectedImage = "";
    $scope.endOfListToRight = false;
    $scope.endOfListToLeft = false;

    var originatorEv;

    $scope.init = function () {
        $scope.getProductAndProductOwner();
        $scope.getMessages();
    }

    $scope.getProductOwner = function () {
        usersService.getUser($scope.product.userName)
            .then(function (user) {
                if (user.data)
                    $scope.productOwner = user.data;
            }, function (error) {
                $scope.doSomethingWithError(error);
            });
    }

    $scope.nextProduct = function () {
        let id = productNavigationService.getNextProduct($scope.product.id);

        if (id === null)
            productNavigationService.getNextProducts()
                .then(function (products) {
                    if (!$scope.checkEndOfListToRight(products))
                        $scope.getProductsForNavigation(products, false);
                });
        else
            $scope.goToProduct(id);
    }

    $scope.checkEndOfListToRight = function (products) {
        if (products === undefined || products.data.products.length === 0) {
            $scope.endOfListToRight = true;
            return true;
        }

        return false;
    };

    $scope.previousProduct = function () {
        let id = productNavigationService.getPreviousProduct($scope.product.id);

        if (id === null)
            productNavigationService.getPreviousProducts()
                .then(function (products) {
                    if (!$scope.checkEndOfListToLeft(products))
                        $scope.getProductsForNavigation(products, true);
                });
        else
            $scope.goToProduct(id);
    }

    $scope.checkEndOfListToLeft = function (products) {
        if (products === undefined || products.data.products.length === 0) {
            $scope.endOfListToLeft = true;
            return true;
        }

        let product = products.data.products[0];

        if (productNavigationService.checkIfIdIsAllreadyAdded(product.id)) {
            $scope.endOfListToLeft = true;
            return true;
        }

        return false;
    };

    $scope.getProductsForNavigation = function (products, inFront) {
        let productIds = products.data.products.map(function (x) { return x.id; });
        inFront === true ? productNavigationService.setProductsIds(productIds, true) : productNavigationService.setProductsIds(productIds, false);
        let id = productNavigationService.getPreviousProduct($scope.product.id);
        $scope.goToProduct(id);
    }

    $scope.goToProduct = function (id) {
        if (id !== undefined)
            $location.url('/product/' + id);
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
        productsService.getProduct($scope.id)
            .then(function (product) {
                if (product.data) {
                    $scope.product = product.data;
                    $scope.selectedImage = $scope.product.images[0] ? $scope.product.images[0].value : "";
                }
            }, function (error) { $scope.doSomethingWithError(error); })
            .then(function () {
                $scope.getProductOwner();
            });
    }

    $scope.getMessages = function () {
        messagesService.getMessagesForProduct($scope.id)
            .then(function (messages) {
                if (messages.data)
                    $scope.messages = messages.data;
            }, function (error) {
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
            }, function (error) {
                $scope.doSomethingWithError(error);
            });
    };

    $scope.saveMessage = function () {
        if (!$scope.newMessage)
            return;

        if ($scope.$parent.token) {
            let message = new Message(null, $scope.$parent.user.userName, $scope.newMessage, Date.now(), $scope.id, false);
            messagesService.postMessageForProduct(message, $scope.$parent.token)
                .then(function (newMessage) {
                    if (newMessage.data)
                        $scope.messages.push(newMessage.data);
                    $scope.newMessage = '';
                }, function (error) { $scope.doSomethingWithError(error); });
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
                }, function (error) { $scope.doSomethingWithError(error) });
        }
    }

    $scope.openImage = function (index) {
        $uibModal.open({
            animation: true,
            templateUrl: 'Site/gallery/gallery.html',
            controller: 'galleryController',
            size: 'lg',
            resolve: {
                images: function () { return $scope.product.images },
                index: function () { return index }
            }
        });
    }

    $scope.setSelected = function (index) {
        $scope.selectedImage = $scope.product.images[index].value;
    };

    $scope.doSomethingWithError = function (error) {
        console.log(error);
    }

    $scope.init();
}