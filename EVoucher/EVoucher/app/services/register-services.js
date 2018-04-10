'use strict';
app.factory('registerServices', ['$http', 'commonServices', function ($http, commonServices) {

    //var serviceBase = 'http://ngauthenticationapi.azurewebsites.net/';

    var registersServiceFactory = {};
    var apiUrl = '/api/e-voucher/';

    var _getRegisters = function (request) {
        var isClaimed = '';
        if (request.isClaimed != 'null') {
            isClaimed = '/' + request.isClaimed;
        }
        var req = {
            method: 'POST',
            url: apiUrl + 'register/list' + isClaimed,
            data: request
        };

        return commonServices.getApiResult(req);
    };

    var _exportRegisters = function (request) {
        var isClaimed = '';
        if (request.isClaimed != 'null') {
            isClaimed = '/' + request.isClaimed;
        }
        var req = {
            method: 'POST',
            url: apiUrl + 'register/export' + isClaimed,
            data: request
        };

        return commonServices.getApiResult(req);
    };

    var _removeRegister = function (register) {
        var req = {
            method: 'POST',
            url: apiUrl + 'register/delete',
            data: JSON.stringify(register)
        };

        return commonServices.getApiResult(req);
    };

    var _submitRegister = function (register) {
        var req = {
            method: 'POST',
            url: apiUrl + 'register/save',
            data: JSON.stringify(register)
        };

        return commonServices.getApiResult(req);
    };

    var _claimProduct = function (claim) {
        var req = {
            method: 'POST',
            url: apiUrl + 'claim',
            data: JSON.stringify(claim)
        };
        return commonServices.getApiResult(req);
    };

    registersServiceFactory.claimProduct = _claimProduct;
    registersServiceFactory.submitRegister = _submitRegister;
    registersServiceFactory.exportRegisters = _exportRegisters;
    registersServiceFactory.getRegisters = _getRegisters;
    registersServiceFactory.removeRegister = _removeRegister;
    return registersServiceFactory;

}]);
