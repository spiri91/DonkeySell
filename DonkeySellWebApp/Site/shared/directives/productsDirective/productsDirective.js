app.directive('productsDirective', ['$location', 'toastr', 'favoritesService', '$uibModal','$timeout', productsDirective]);

function productsDirective($location, toastr, favoritesService, $uibModal, $timeout) {
    return {
        restrict: 'A',
        templateUrl: 'Site/shared/directives/productsDirective/productsDirective.html',
        scope: true,
        link: function (scope, elem, attrs) {
            var modalInstance;
            scope.addToFavorites = function (product, $event) {
                $event.stopPropagation();
                if (!scope.$parent.$parent.token) {
                    toastr.error('Please login first!');
                    return;
                }

                if (scope.$parent.$parent.favorites.filter(function (x) { return x.id === product.id }).length > 0) {
                    toastr.error('This product is allready added!');
                    return;
                }

                favoritesService.postProductToFavorites(scope.$parent.$parent.user.userName, scope.$parent.$parent.token, product.id)
                    .then(function () {
                        scope.$parent.$parent.favorites.push(product);
                        toastr.success('Product added to Favorites!');
                    }, function (error) {
                        console.log(error);
                    });
            };
        }
    };
};