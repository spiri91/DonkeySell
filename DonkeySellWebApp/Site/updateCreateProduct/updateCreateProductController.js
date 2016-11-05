app.controller('updateCreateProductController', ['$scope', 'productsService', '$location', 'toastr', 'usersService', '$routeParams', 'othersService', '$mdDialog', updateCreateProductController]);

function updateCreateProductController($scope, productsService, $location, toastr, usersService, $routeParams, otherService, $mdDialog) {
    $scope.description = '';
    $scope.city = '';
    $scope.product = {};
    $scope.cities = [];
    $scope.categories = [];
    $scope.selectedImage = "";
    $scope.user = $scope.$parent.user;

    $scope.userName = $scope.$parent.username;
    $scope.productId = $routeParams.productId;

    $scope.saveProduct = function () {
        if ($scope.$parent.token) {
            let token = $scope.$parent.token;

            productsService.addProduct($scope.product, token)
                .then(function () {
                    toastr.success('Product successfully edited!');
                    $location.url('/home');
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
            productsService.getProduct($scope.productId).then(function (product) {
                $scope.product = product.data;
                $scope.selectedImage = $scope.product.images[0].value;
            });
        }

        $scope.getCategoriesAndCities();
    }

    $scope.getCategoriesAndCities = function () {
        otherService.getCategories()
            .then(function (categories) {
                $scope.categories = categories.data;
            });

        otherService.getCities()
            .then(function (cities) {
                $scope.cities = cities.data;
            });
    };
  
    $scope.addImageToProduct = function(image) {
        let newImage = new Image(null, image);
        $scope.product.images.push(newImage);

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

        angular.forEach(files,
            function(flowFile, i) {
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

    $scope.init();
}