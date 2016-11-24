app.controller('settingsController', ['$scope', 'storageService', settingsController]);

function settingsController($scope, storageService) {
    $scope.useCustomBackground = false;
    $scope.useFixedNavbar = false;

    $scope.useFixedNavbarId = "useFixedNavbar";
    $scope.backgroundImageId = 'backgroundImage';
    $scope.useBackgroundId = "useBackground";



    $scope.setBackgroundImage = function () {
        if ((document.getElementById('backgroundImage')).files[0]) {
            let file = (document.getElementById('backgroundImage')).files[0];
            let reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = function () {
                let image = reader.result;
                $scope.setInStorage($scope.backgroundImageId, image);
                let url = 'url(' + image + ')';
                $scope.applyImageBackground(url);
                $scope.setInStorage($scope.useBackgroundId, 'true');
            }
        }
    };

    $scope.init = function () {
        $scope.setBackgroundImagePreference();
        $scope.setNavbarPreference();
    }

    $scope.applyImageBackground = function (imageUrl) {
        $('#body').css("background-image", imageUrl);
    }

    $scope.changeBackgroundPreference = function() {
        let value = $scope.useCustomBackground.toString();
        if (value === 'true') {
            $scope.useCustomBackground = true;
            $scope.setInStorage($scope.useBackgroundId, 'true');
            document.getElementById('backgroundImage').click();
        } else {
            $scope.useCustomBackground = false;
            $scope.applyImageBackground('none');
            $scope.setInStorage($scope.useBackgroundId, 'false');
        }
    }

    $scope.setBackgroundImagePreference = function () {
        let useCustomBackground = $scope.getFromStorage($scope.useBackgroundId);
        if (useCustomBackground === 'true') {
            $scope.useCustomBackground = true;
            let image = $scope.getFromStorage($scope.backgroundImageId);
            let url = 'url(' + image + ')';
            $scope.applyImageBackground(url);
        } else {
            $scope.useCustomBackground = false;
            $scope.applyImageBackground('none');
        }
    }

    $scope.setNavbarPreference = function () {
        let useFixed = $scope.getFromStorage($scope.useFixedNavbarId);
        if (useFixed === 'true') {
            $('#navbar').addClass('navbar-fixed-top');
            $('#body').addClass('added-margin-top');
            $scope.useFixedNavbar = true;
        } else {
            $('#navbar').removeClass('navbar-fixed-top');
            $('#body').removeClass('added-margin-top');
            $scope.useFixedNavbar = false;
        }
    }

    $scope.changeNavbarPreference = function () {
        let value = $scope.useFixedNavbar.toString();
        $scope.setInStorage($scope.useFixedNavbarId, value);
        $scope.setNavbarPreference();
    }

    $scope.setInStorage = function (id, value) {
        storageService.set(id, value);
    }

    $scope.getFromStorage = function (id) {
        let value = storageService.get(id);

        return value;
    }

    $scope.init();
}