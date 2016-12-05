app.directive('loadingSpecific',
[
    function loadingSpecific() {
        return {
            restrict: 'A',
            templateUrl: 'Site/shared/directives/loadingSpecificDirective/loadingSpecific.html'
        }
    }
]);