﻿'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = '/';
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        userName: "",
        userId: '',
        token: '',
        useRefreshTokens: false,
        avatar: "",
        referredUrl: '/'
    };

    var _externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: ""
    };

    var _saveRegistration = function (registration) {

        _logOut();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    var _login = function (loginData) {

        //var data = "grant_type=password&email=" + loginData.userName + "&deviceId=" + loginData.password;
        var data = {
            Email: loginData.userName,
            DeviceId: loginData.password,
            NotificationId: null,
            Password: null
        };
        //if (loginData.useRefreshTokens) {
        //    data = data + "&client_id=" + ngAuthSettings.clientId;
        //}

        var deferred = $q.defer();

        $http.post(serviceBase + 'api/account/login', data).success(function (response) {

            localStorageService.set('authorizationData', { token: response.data.access_token, userName: response.data.userData.NickName, roleNames: response.data.userData.RoleNames, avatar: response.data.userData.Avatar, refresh_token: response.data.refresh_token, userId: response.data.userData.Id });
            _authentication.isAuth = true;
            _authentication.isAdmin = $.inArray("Admin", response.data.userData.RoleNames) >= 0;
            _authentication.userName = response.data.userData.NickName;
            _authentication.roleNames = response.data.userData.RoleNames;
            _authentication.userId = response.data.userData.Id;
            _authentication.avatar = response.data.userData.Avatar;
            _authentication.useRefreshTokens = loginData.useRefreshTokens;
            _authentication.token = response.data.access_token;
            _authentication.refresh_token = response.data.refresh_token;
            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function () {

        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.isAdmin = false;
        _authentication.userName = "";
        _authentication.useRefreshTokens = false;

    };

    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.isAdmin = $.inArray("Admin", authData.roleNames) >= 0;
            _authentication.userName = authData.userName;
            _authentication.roleNames = authData.roleNames;
            _authentication.userId = authData.userId;
            _authentication.avatar = authData.avatar;

            _authentication.token = authData.token;
            _authentication.refresh_token = authData.refresh_token;
        }

    };

    var _refreshToken = function () {
        var deferred = $q.defer();

        var authData = localStorageService.get('authorizationData');

        if (authData) {

            if (authData.useRefreshTokens) {

                var data = "grant_type=refresh_token&refresh_token=" + authData.refresh_token + "&client_id=" + ngAuthSettings.clientId;

                localStorageService.remove('authorizationData');

                $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                    localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refresh_token: response.refresh_token, useRefreshTokens: true });

                    deferred.resolve(response);

                }).error(function (err, status) {
                    _logOut();
                    deferred.reject(err);
                });
            }
        }

        return deferred.promise;
    };

    var _obtainAccessToken = function (externalData) {

        var deferred = $q.defer();

        $http.get(serviceBase + 'api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } }).success(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, roleName: response.userData.roleNames, refresh_token: response.refresh_token, useRefreshTokens: true });

            _authentication.isAuth = true;
            _authentication.isAdmin = _authentication.isAdmin = $.inArray("Admin", response.userData.RoleNames) >= 0;
            _authentication.userName = response.userName;
            _authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _registerExternal = function (registerExternalData) {

        var deferred = $q.defer();

        $http.post(serviceBase + 'api/account/registerexternal', registerExternalData).success(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refresh_token: response.refresh_token, useRefreshTokens: true });

            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            _authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    authServiceFactory.refreshToken = _refreshToken;

    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    authServiceFactory.externalAuthData = _externalAuthData;
    authServiceFactory.registerExternal = _registerExternal;

    return authServiceFactory;
}]);