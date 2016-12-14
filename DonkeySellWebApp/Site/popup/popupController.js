app.controller('popupController', ['$scope', ngPopupConfig]);

function ngPopupConfig($scope) {
    $scope.id = $scope.$parent.product;
};