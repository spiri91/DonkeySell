app.controller('popupController', ['$scope', 'productsService', 'id', '$uibModalInstance', popupController]);

function popupController($scope, productsService, id, $uibModalInstance) {
    $scope.id = id;
    $scope.product = {};
    $scope.loading = true;

    $scope.init = function () {
        productsService.getProduct($scope.id)
            .then(function(product) {
                    if (product.data)
                        $scope.product = product.data;
                },
                function(error) {
                    console.log(error);
                })
            .finally(function() {
                $scope.loading = false;
            });

    }

    $scope.close = function() {
        $uibModalInstance.close();
    };

    $scope.init();
}