angular.module('oomApp', [
    'ngRoute',
    'ngAnimate',
    'angular-loading-bar',
    'ui.bootstrap',
    'oomApp.controllers.home'
])

    .config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {
        $routeProvider
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
   }])
    
    .run(['$rootScope', '$route', function ($rootScope, $route) {
        $rootScope.$on('$routeChangeSuccess', function (newVal, oldVal) {
            if (oldVal !== newVal) {
                document.title = $route.current.title;
            }
        });
    }])
    
;