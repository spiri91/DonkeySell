app.controller('contactPageController', ['$scope','$mdBottomSheet', 'Item', '$mdDialog', contactPageController]);

function contactPageController($scope, $mdBottomSheet, Item, $mdDialog) {
    $scope.item = Item;

    $scope.showInfo = function(infoFor, info) {
        $mdDialog.show(
            $mdDialog.alert()
            .parent(angular.element(document.querySelector('#popupContainer')))
            .clickOutsideToClose(true)
            .title(infoFor)
            .textContent(info)
            .ok('Got it!')
        );
    }
}