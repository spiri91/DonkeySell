app.controller('improvementsController', ['$scope', 'improvementsService', 'toastr', improvementsController]);

function improvementsController($scope, improvementsService, toastr) {
    $scope.minimize = false;
    $scope.open = true;
    $scope.suggestion = "";

    $scope.closeWindow = function () {
        $scope.open = false;
    }

    $scope.minimizeWindow = function () {
        $scope.minimize = !$scope.minimize;
    }

    $scope.sendSuggestion = function () {
        if (!$scope.suggestion.length > 5)
            return;

        let improvement = new Improvement();
        improvement.value = $scope.suggestion;

        improvementsService.postImprovement(improvement)
            .then(function () {
                    toastr.success("Thank you! :)");
                },
                function (error) {
                    console.log(error);
                })
        .finally(function() {
                $scope.suggestion = "";
            });
    }
};