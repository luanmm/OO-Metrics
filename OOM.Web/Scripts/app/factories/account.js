angular.module('oomApp.factories.account', [])

    .factory('AuthHttpResponseInterceptor', ['$q', '$location', function ($q, $location) {
        return {
            response: function (response) {
                if (response.status === 401) {
                    console.log("Response 401");
                }
                return response || $q.when(response);
            },
            responseError: function (rejection) {
                if (rejection.status === 401) {
                    console.log("Response Error 401", rejection);
                    $location.path('/login').search('returnUrl', $location.path());
                }
                return $q.reject(rejection);
            }
        };
    }])

    .factory('LoginFactory', ['$http', '$q', function ($http, $q) {
        return function (emailAddress, password, rememberMe) {

            var deferredObject = $q.defer();

            $http.post(
                '/Account/Login', {
                    Email: emailAddress,
                    Password: password,
                    RememberMe: rememberMe
                }
            ).
            success(function (data) {
                if (data == "True") {
                    deferredObject.resolve({ success: true });
                } else {
                    deferredObject.resolve({ success: false });
                }
            }).
            error(function () {
                deferredObject.resolve({ success: false });
            });

            return deferredObject.promise;
        };
    }])

    .factory('RegistrationFactory', ['$http', '$q', function ($http, $q) {
        return function (emailAddress, password, confirmPassword) {
            var deferredObject = $q.defer();

            $http.post(
                '/Account/Register', {
                    Email: emailAddress,
                    Password: password,
                    ConfirmPassword: confirmPassword
                }
            ).
            success(function (data) {
                if (data == "True") {
                    deferredObject.resolve({ success: true });
                } else {
                    deferredObject.resolve({ success: false });
                }
            }).
            error(function () {
                deferredObject.resolve({ success: false });
            });

            return deferredObject.promise;
        };
    }])

;