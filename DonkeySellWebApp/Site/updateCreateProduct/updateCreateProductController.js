app.controller('updateCreateProductController', ['$scope', 'productsService', '$location', 'toastr', 'usersService', '$routeParams', '$mdDialog', updateCreateProductController]);

function updateCreateProductController($scope, productsService, $location, toastr, usersService, $routeParams, $mdDialog) {
    $scope.description = '';
    $scope.city = '';
    $scope.product = {};
    $scope.cities = [];
    $scope.categories = [];
    $scope.selectedImage = "";
    $scope.user = $scope.$parent.user;
    $scope.errors = [];

    $scope.userName = $scope.$parent.username;
    $scope.productId = $routeParams.productId;

    $scope.saveProduct = function () {
        if ($scope.$parent.token) {
            let token = $scope.$parent.token;

            productsService.addProduct($scope.product, token)
                .then(function () {
                    toastr.success('Product successfully edited!');
                    $location.url('/home');
                }, function(error) {
                    toastr.error('An error occured!');
                    $scope.doSomethingWithError(error);
                });
        } else
            toastr.error('Please logIn first!');
    };

    $scope.init = function () {
        if ($scope.productId === '0') {
            $scope.product = new Product();
            $scope.product.userName = $scope.user.userName;
            $scope.product.userMail = $scope.user.email;
            $scope.product.userId = $scope.user.userId;
            $scope.product.images = [];
        } else {
            productsService.getProduct($scope.productId).then(function(product) {
                if (product.data) {
                    $scope.product = product.data;
                    $scope.selectedImage = $scope.product.images[0].value;
                }
            }, function(error) {
                toastr.error("An error occured");
                $scope.doSomethingWithError(error);
            });
        }

        $scope.getCategoriesAndCities();
    }

    $scope.getCategoriesAndCities = function () {
        $scope.categories = $scope.$parent.categories;
        $scope.cities = $scope.$parent.cities;
    };
  
    $scope.addImageToProduct = function (image) {
        let newImage = new Image(null, image);
        $scope.$apply(function() {
            $scope.product.images.push(newImage);
        });

        if (!$scope.selectedImage)
            $scope.selectedImage = image;
    }

    $scope.showAlert = function () {
        $mdDialog.show(
              $mdDialog.alert()
                .clickOutsideToClose(true)
                .title('Warning')
                .textContent('You allready have added 3 images. Remove the ones you want to replace')
                .ok('Got it!')
        );
    }

    $scope.removeImage = function(image) {
        let index = $scope.product.images.indexOf(image);
        $scope.product.images.splice(index, 1);

        if ($scope.selectedImage === image.value)
            $scope.selectedImage = "";
    }

    $scope.processFiles = function (files) {
        if ($scope.product.images.length >= 3) {
            $scope.showAlert();
            return;
        }

        for (let i = 0; i < files.length; i++) {
            let shouldExit = false;
            if (files[i].size > 2 * 1024 * 1024) {
                $scope.errors.push(files[i].name + " is to big! :(");
                shouldExit = true;
            }
            if (shouldExit === true)
                return;
        }

        angular.forEach(files,
            function(flowFile) {
                var fileReader = new FileReader();
                fileReader.onload = function(event) {
                    var uri = event.target.result;
                    $scope.addImageToProduct(uri);
                };
                fileReader.readAsDataURL(flowFile.file);
            });
    }

    $scope.setMainImage = function (image) {
        $scope.selectedImage = image.value;
    }

    $scope.doSomethingWithError = function(error) {
        console.log(error);
    }

    $scope.init();
}