app.controller("galleryController", ['$scope', '$uibModalInstance','images', 'index', galleryController]);

function galleryController($scope, $uibModalInstance, images, index) {
    $scope.images = images;
    $scope.index = index;
    $scope.selectedImage = {};

    $scope.init = function() {
        $scope.selectedImage = $scope.images[$scope.index];
    }

    $scope.cancel = function () {
        $uibModalInstance.close();
    }

    $scope.showNext = function() {
        if ($scope.index === $scope.images.length-1)
            $scope.index = 0;
        else 
            $scope.index++;

        $scope.init();
    }

    $scope.showPrevious = function() {
        if ($scope.index === 0)
            $scope.index = $scope.images.length - 1;
        else
            $scope.index--;

        $scope.init();
    }

    $scope.init();
};