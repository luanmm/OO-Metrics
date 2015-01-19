angular.module('oomApp.controllers.account', [
    'ngRoute',
    'oomApp.factories.account'
])

    .controller('LoginController', ['$scope', '$routeParams', '$location', 'LoginFactory', function ($scope, $routeParams, $location, LoginFactory) {
        $scope.loginForm = {
            emailAddress: '',
            password: '',
            rememberMe: false,
            returnUrl: $routeParams.go,
            loginFailure: false
        };

        $scope.login = function () {
            var result = LoginFactory($scope.loginForm.emailAddress, $scope.loginForm.password, $scope.loginForm.rememberMe);
            result.then(function (result) {
                if (result.success) {
                    if ($scope.loginForm.returnUrl === undefined) {
                        $location.path('/projects');
                    } else {
                        $location.path($scope.loginForm.returnUrl);
                    }
                } else {
                    $scope.loginForm.loginFailure = true;
                }
            });
        };
    }])

    .controller('RegisterController', ['$scope', '$location', 'RegistrationFactory', function ($scope, $location, RegistrationFactory) {
        $scope.registerForm = {
            emailAddress: '',
            password: '',
            confirmPassword: ''
        };

        $scope.register = function () {
            var result = RegistrationFactory($scope.registerForm.emailAddress, $scope.registerForm.password, $scope.registerForm.confirmPassword);
            result.then(function (result) {
                if (result.success) {
                    $location.path('/routeOne');
                } else {
                    $scope.registerForm.registrationFailure = true;
                }
            });
        };
    }])

;