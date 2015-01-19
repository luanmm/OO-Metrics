angular.module('oomApp', [
    'ngRoute',
    'ui.bootstrap',
    'oomApp.controllers.account',
    'oomApp.controllers.home',
    'oomApp.factories.account'
])

    .config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
        $routeProvider
            .when('/login', {
                title: 'Login',
                templateUrl: 'Account/Login',
                controller: 'LoginController'
            })
            .when('/register', {
                title: 'Register',
                templateUrl: 'Account/Register',
                controller: 'RegisterController'
            })
            .when('/', {
                title: 'Home',
                templateUrl: 'Home/Index',
                controller: 'HomeController'
            })
            .when('/projects', {
                title: 'Projects',
                templateUrl: 'Projects/Index'
            })
            .when('/metrics', {
                title: 'Metrics',
                templateUrl: 'Metrics/Index'
            })
            .otherwise({
                redirectTo: '/'
            });

        $httpProvider.interceptors.push('AuthHttpResponseInterceptor');
    }])
    
    .run(['$rootScope', '$route', function ($rootScope, $route) {
        $rootScope.$on('$routeChangeSuccess', function (newVal, oldVal) {
            if (oldVal !== newVal) {
                document.title = $route.current.title;
            }
        });
    }])
    
;