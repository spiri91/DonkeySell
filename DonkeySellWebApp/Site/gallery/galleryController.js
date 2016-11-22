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
        
    }

    $scope.showPrevious = function() {
        
    }

    $scope.init();
};