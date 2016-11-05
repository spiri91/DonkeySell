app.directive('productsDirective', ['$location', 'toastr', 'favoritesService', productsDirective]);

function productsDirective($location, toastr, favoritesService) {
    return {
        restrict: 'A',
        templateUrl: 'Site/shared/directives/productsDirective/productsDirective.html',
        scope: true,
        link: function (scope, elem, attrs) {
            scope.goToProduct = function (id) {
                $location.url('/product/' + id);
            };

            scope.addToFavorites = function (product) {
                if (!scope.$parent.$parent.token) {
                    toastr.error('Please login first!');
                    return;
                }

                if (scope.$parent.$parent.favorites.indexOf(product) >= 0) {
                    toastr.error('This product is allready added!');
                    return;
                }

                favoritesService.postProductToFavorites(scope.$parent.$parent.user.userName, scope.$parent.$parent.token, product.id)
                    .then(function () {
                        scope.$parent.$parent.favorites.push(product);
                        toastr.success('Product added to Favorites!');
                    });
            };
        }
    };
};