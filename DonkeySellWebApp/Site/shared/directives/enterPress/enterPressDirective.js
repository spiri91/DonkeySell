app.directive('onKeyEnter', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.bind('keydown keypress', function (event) {
                if (event.keyCode === 13) {
                    var attrValue = $parse(attrs.onKeyEnter);
                    (typeof attrValue === 'function') ? attrValue(scope) : angular.noop();
                    event.preventDefault();
                }
            });
            scope.$on('$destroy',
                function() {
                    element.unbind('keydown keypress');
                });
        }
    };
}]);