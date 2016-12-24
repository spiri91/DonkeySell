app.controller('updateCreateProductController', ['$scope', 'productsService', '$location', 'toastr', 'usersService', '$routeParams',
    '$mdDialog', 'uiGmapGoogleMapApi', '$rootScope', updateCreateProductController]);

function updateCreateProductController($scope, productsService, $location, toastr, usersService, $routeParams, $mdDialog, uiGmapGoogleMapApi, $rootScope) {
    $scope.description = '';
    $scope.city = '';
    $scope.product = {};
    $scope.cities = [];
    $scope.categories = [];
    $scope.selectedImage = "";
    $scope.user = $scope.$parent.user;
    $scope.errors = [];
    $scope.mapReady = false;
    $scope.loading = false;

    $scope.marker = {};
    $scope.map = {};

    $scope.userName = $scope.$parent.username;
    $scope.productId = $routeParams.productId;
    $scope.edit = false;

    $scope.saveProduct = function () {
        if ($scope.$parent.token) {
            $scope.loading = true; 
            let token = $scope.$parent.token;

            productsService.addProduct($scope.product, token)
                .then(function () {
                    let message = $scope.edit === true ? 'Product successfully edited!' : 'Product saved';
                    toastr.success(message);
                    $scope.loading = false;
                    $location.url('/home');
                }, function (error) {
                    toastr.error('An error occured!');
                    $scope.doSomethingWithError(error);
                });
        } else
            toastr.error('Please login first!');
    };

    $scope.init = function () {
        if ($scope.productId === '0') {
            $scope.product = new Product();
            $scope.product.userName = $scope.user.userName;
            $scope.product.userMail = $scope.user.email;
            $scope.product.userId = $scope.user.userId;
            $scope.product.images = [];
            $scope.setCoordinates();
        } else {
            $scope.edit = true;
            $scope.loading = true;
            productsService.getProduct($scope.productId).then(function (product) {
                if (product.data) {
                    $scope.product = product.data;
                    $scope.selectedImage = $scope.product.images.length > 0 ? $scope.product.images[0].value : '';
                    if ($scope.product.meetingPoint) {
                        let coordinates = $scope.product.meetingPoint.split(';');
                        $scope.latitude = coordinates[0];
                        $scope.longitude = coordinates[1];
                        $scope.setCoordinatesForMarkerAndMap();
                    } else
                        $scope.setCoordinates();
                }
            }, function (error) {
                toastr.error("An error occured");
                $scope.doSomethingWithError(error);
            }).finally(() => {
                $scope.loading = false;
            });
        }

        $scope.getCategoriesAndCities();
    }

    $scope.getCategoriesAndCities = function () {
        if ($scope.$parent.categories.length > 0 && $scope.$parent.cities.length > 0) {
            $scope.categories = $scope.$parent.categories;
            $scope.cities = $scope.$parent.cities;
        } else {
            $rootScope.$on('citiesAndCategoriesLoaded',
            () => {
                $scope.categories = $scope.$parent.categories;
                $scope.cities = $scope.$parent.cities;
            });
        }
    };

    $scope.addImageToProduct = function (image) {
        let newImage = new Image(null, image);
        $scope.$apply(function () {
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

    $scope.removeImage = function (image) {
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
            function (flowFile) {
                var fileReader = new FileReader();
                fileReader.onload = function (event) {
                    var uri = event.target.result;
                    $scope.addImageToProduct(uri);
                };
                fileReader.readAsDataURL(flowFile.file);
            });
    }

    $scope.setMainImage = function (image) {
        $scope.selectedImage = image.value;
    }

    $scope.doSomethingWithError = function (error) {
        console.log(error);
    }

    $scope.setCoordinates = function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                $scope.latitude = position.coords.latitude;
                $scope.longitude = position.coords.longitude;
                $scope.setCoordinatesForMarkerAndMap();
            });
        }
    }

    $scope.setCoordinatesForMarkerAndMap = function () {
        uiGmapGoogleMapApi.then(function () {
            $scope.marker = {
                id: 0,
                options: { draggable: true },
                coords: {
                    latitude: $scope.latitude,
                    longitude: $scope.longitude
                },
                events: {
                    dragend: function (marker) {
                        let lat = marker.getPosition().lat();
                        let lon = marker.getPosition().lng();
                        $scope.product.meetingPoint = lat + ";" + lon;
                    }
                }
            };

            $scope.map = {
                center: {
                    latitude: $scope.latitude,
                    longitude: $scope.longitude
                },
                zoom: 14
            };

            $scope.mapReady = true;
        });
    }

    $scope.init();
}