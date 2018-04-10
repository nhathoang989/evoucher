﻿'use strict';
app.controller('registerController', ['$scope', '$rootScope', '$timeout', '$location', 'authService', 'registerServices', function ($scope, $rootScope, $timeout, $location, authService, registerServices) {
    $scope.errors = [];
    $scope.selected = {};
    $rootScope.page = 'page-home';
    $scope.downloadLink = '';
    $scope.dateRegexp = new RegExp('\d{4}-\d{2}-\d{2}');
    $scope.register = {
        fullname: '',
        phone: '', // unique
        manufacturer: '',
        automaker: '',
        carModel: '', //2015, 2016
        license: ''
    }
    $scope.isValid = true;
    $scope.isBusy = false;

    $('.side-nav li').removeClass('active');
    $('.side-nav .page-registers').addClass('active');
    $scope.data = [];
    $scope.claim = {
        name: '',
        quantity: 0,
        codeEvoucher: ''
    };
    $scope.dateRange = {
        fromDate: null,
        toDate: null
    }
    $scope.request = {
        keyword: '',
        pageIndex: 0,
        pageSize: 'null',
        fromDate: null,
        toDate: null,
        orderBy: 'CreatedDate',
        direction: '1',
        isClaimed: 'null'
    };
    $scope.range = function (max) {
        var input = [];
        for (var i = 1; i <= max; i += 1) input.push(i);
        return input;
    };

    $scope.errors = []
    $scope.validate = function () {

    };
    $scope.openSearch = function () {
        $('#dlg-search').modal("show");
    };
    $scope.loadRegister = function (register) {
        $scope.selected = register;
        $scope.errors = []
        $('#dlg-register').modal("show")
    };
    $scope.initRegister = function () {
        $.ajax({
            method: 'GET',
            contentType: "application/json; charset=utf-8",
            url: 'api/e-voucher/init-register',
            success: function (data) {
                $scope.register = data;
                console.log($scope.register);
            },
            error: function (a, b, c) {
                console.log(a + " " + b + " " + c);
                $scope.isBusy = false;
                $scope.$apply();
            }
        });
    }

    $scope.submitRegister = function () {
        if (!$scope.isValid) {
            $scope.errors.push('Thông tin không hợp lệ');
        }
        else {
            if (!$scope.isBusy) {
                $scope.isBusy = true;
                registerServices.submitRegister($scope.register).then(function (results) {
                    var resp = results.data;
                    if (resp.isSucceed) {
                        location.href = "/thank-you";
                    }
                    else {
                        $scope.errors = resp.errors;
                    }
                    $scope.isBusy = false;
                }, function (error) {
                    $scope.isBusy = false;
                    //alert(error.data.message);
                });
            }
        }
    }

    $scope.claimProduct = function () {
        if (!$scope.isBusy) {

            $scope.errors = [];
            $scope.isBusy = true;
            $scope.claim.codeVoucher = $scope.selected.code;
            registerServices.claimProduct($scope.claim).then(function (results) {
                var resp = results.data;
                if (resp.isSucceed) {

                    $scope.claim = {
                        name: '',
                        quantity: 0,
                        codeEvoucher: ''
                    };

                    $('#dlg-register').modal('hide');
                    $scope.isBusy = false;
                    $scope.loadRegisters();
                }
                else {
                    $scope.isBusy = false;
                    $scope.errors = resp.errors;
                }

            }, function (error) {
                $scope.isBusy = false;
                //alert(error.data.message);
            });
        }
    }


    $scope.loadRegisters = function (pageIndex = 0, btn) {
        $scope.isValid = $scope.request.keyword == '' || $.isNumeric($scope.request.keyword)
        if (!$scope.isBusy && $scope.isValid) {
            
            $scope.isBusy = true;
            if (!pageIndex) {
                $scope.request.pageIndex = pageIndex;
            }
            if (Date.parse($scope.dateRange.fromDate)) {
                $scope.request.fromDate = new Date($scope.dateRange.fromDate).toISOString();
            }
            else {
                $scope.request.fromDate = null;
            }
            if (Date.parse($scope.dateRange.toDate)) {
                $scope.request.toDate = new Date($scope.dateRange.toDate).toISOString();
            }
            else {
                $scope.request.toDate = null;
            }
            registerServices.getRegisters($scope.request).then(function (results) {
                var resp = results.data;
                if (resp.isSucceed) {
                    $scope.data = resp.data;
                    $scope.downloadLink = '';
                    $('#dlg-search').modal("hide");
                }
                $scope.isBusy = false;
            }, function (error) {
                $scope.isBusy = false;
                //alert(error.data.message);
            });
        }
    }
    $scope.exportRegisters = function (pageIndex = 0, btn) {
        if (!$scope.isBusy) {
            $scope.isBusy = true;
            if (!pageIndex) {
                $scope.request.pageIndex = pageIndex;
            }
            if (Date.parse($scope.dateRange.fromDate)) {
                $scope.request.fromDate = new Date($scope.dateRange.fromDate).toISOString();
            }
            else {
                $scope.request.fromDate = null;
            }
            registerServices.exportRegisters($scope.request).then(function (results) {
                var resp = results.data;
                if (resp.isSucceed) {
                    $scope.downloadLink = resp.data;
                }
                $scope.isBusy = false;
            }, function (error) {
                $scope.isBusy = false;
                //alert(error.data.message);
            });
        }
    }
    $scope.removeRegister = function (register) {
        if (confirm("Remove this register ?")) {
            registerServices.removeRegister(register).then(function (results) {
                if (results.data.isSucceed) {
                    $scope.loadRegisters();
                }
            }, function (error) {

            });
        }


    }
}]);