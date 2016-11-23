app.controller('settingsController', ['$scope', 'storageService', 'toastr', settingsController]);

function settingsController($scope, storageService, toastr) {
    $scope.useCustomBackground = false;

    $scope.backgroundImageName = 'backgroundImage';
    $scope.useBackground = "useBackground";

    $scope.setBackgroundImage = function () {
        if ((document.getElementById('backgroundImage')).files[0]) {
            let file = (document.getElementById('backgroundImage')).files[0];
            let reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = function () {
                let image = reader.result;
                let url = 'url(' + image + ')';
                $scope.postImageInStorage(image);
                $scope.applyImageBackground(url);
                storageService.set($scope.useBackground, 'true');
            }
        }
    };

    $scope.init = function () {
        let useBackground = storageService.get($scope.useBackground);
        if (useBackground === 'true') {
            $scope.useCustomBackground = true;
            let image = $scope.getImageFromStorage();
            let url = 'url(' + image + ')';
            $scope.applyImageBackground(url);
        }
    }

    $scope.postImageInStorage = function (image) {
        storageService.set($scope.backgroundImageName, image);
    }

    $scope.getImageFromStorage = function () {
        let image = storageService.get($scope.backgroundImageName);
        return image;
    }

    $scope.applyImageBackground = function (imageUrl) {
        $('#body').css("background-image", imageUrl);
    }

    $scope.setUseBackground = function () {
        if ($scope.useCustomBackground === false) {
            $scope.applyImageBackground('none');
            storageService.set($scope.useBackground, 'false');
            return;
        }

        storageService.set($scope.useBackground, 'true');
        $scope.init();
    }

    $scope.init();
}