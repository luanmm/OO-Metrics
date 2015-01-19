angular.module('oomApp.controllers.home', [])

    .controller('HomeController', ['$scope', function ($scope) {
        $scope.models = {
            helloAngular: 'I work!'
        };
    }])

;