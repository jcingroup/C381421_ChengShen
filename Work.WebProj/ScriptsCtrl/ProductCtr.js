var apiProductData = gb_approot + 'api/ProductData';
angular.module('angularApp', ['commfun', 'ui.bootstrap', 'ngCkeditor', 'toaster', 'commbox']).controller('ctrl', ['$scope', '$http', '$modal', 'toaster', 'gridpage', function ($scope, $http, $modal, toaster, gridpage) {
    $scope.editorOptions = CKConfig;
    $scope.show_master_edit = false;
    $scope.show_detail_edit = false;
    $scope.edit_type = 0 /* none */;
    var timer = false;
    $scope.grid_new_show = true;
    $scope.grid_del_show = false;
    $scope.grid_nav_show = true;
    $scope.check_del_value = false;
    $scope.JumpPage = function (page) {
        $scope.NowPage = page;
        gridpage.CountPage($scope);
    };
    $scope.JumpPageKey = function () {
        gridpage.CountPage($scope);
    };
    $scope.Init_Query = function () {
        gridpage.CountPage($scope);
    };
    $scope.ExpandSub = function ($index) {
        $scope.Grid_Items[$index].is_show = !$scope.Grid_Items[$index].is_show;
        if ($scope.Grid_Items[$index].is_show) {
            $scope.Detail_Init($index);
        }
    };
    $scope.Master_Grid_Delete = function () {
        var ids = [];
        for (var key in $scope.Grid_Items) {
            if ($scope.Grid_Items[key].check_del) {
                ids.push('ids=' + $scope.Grid_Items[key].id);
            }
        }
        if (ids.length > 0) {
            if (confirm(msg_Info_Is_SureDelete)) {
                $http['delete'](apiConnection + '?' + ids.join('&')).success(function (data, status, headers, config) {
                    if (data.result) {
                        $scope.Init_Query();
                    }
                    else {
                        toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                    }
                });
            }
        }
        else {
            alert(msg_Warn_Select_Delete_Data);
        }
    };
    $scope.SelectAllCheckDel = function ($event) {
        for (var key in $scope.Grid_Items) {
            $scope.Grid_Items[key].check_del = $event.target.checked;
            console.log($scope.Grid_Items[key].check_del);
        }
    };
    $scope.Master_Submit = function () {
        if ($scope.edit_type == 1 /* insert */) {
            console.info("Insert Mode Start...");
            $http.post(gb_approot + apiConnection, $scope.fd).success(function (data, status, headers, config) {
                if (data.result) {
                    $scope.edit_type = 2 /* update */;
                    toaster.pop('success', js_Info_Toast_Return_Title, Info_Insert_Success);
                    $scope.Init_Query();
                    console.info("Insert Finish", $scope.edit_type);
                }
                else {
                    toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                }
            });
        }
        if ($scope.edit_type == 2 /* update */) {
            console.info("Update Mode Start...");
            $http.put(gb_approot + apiConnection, $scope.fd).success(function (data, status, headers, config) {
                if (data.result) {
                    toaster.pop('success', js_Info_Toast_Return_Title, Info_Update_Success);
                    $scope.Init_Query();
                    console.info("Update Finish");
                }
                else {
                    toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                }
            });
        }
    };
    $scope.Master_Edit_Close = function () {
        $scope.edit_type = 0 /* none */;
        $scope.show_master_edit = false;
        $scope.fd = {};
    };
    $scope.Master_Open_Modify = function ($index) {
        var get_id = $scope.Grid_Items[$index].id;
        console.log('Start get master modify data', get_id);
        $http.get(gb_approot + apiConnection, { params: { id: get_id } }).success(function (data, status, headers, config) {
            if (data.result) {
                data.data = obj_prop_date(data.data);
                $scope.fd = data.data;
                $scope.show_master_edit = true;
                $scope.edit_type = 2 /* update */;
                console.info("Master data get ok!", "Show", $scope.show_master_edit);
            }
            else {
                console.error(data.message);
                toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
            }
        });
    };
    $scope.Master_Open_New = function () {
        $scope.fd = {};
        $http.post(aj_MGetId, {}).success(function (data, status, headers, config) {
            $scope.fd = {
                id: data
            };
            $scope.edit_type = 1 /* insert */;
            console.info("Get New Id Value:", data);
        });
        $scope.show_master_edit = true;
    };
    $scope.Detail_Init = function ($index) {
        var get_id = $scope.Grid_Items[$index].id;
        $http.get(apiProductData, { params: { type_id: get_id } }).success(function (data, status, headers, config) {
            if (data.result) {
                $scope.Grid_Items[$index].productData = data.data;
            }
            else {
                console.error(data.message);
                toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
            }
            ;
        });
    };
    $scope.Sub_Detail_Delete = function ($index) {
        var ids = [];
        for (var key in $scope.Grid_Items[$index].productData) {
            if ($scope.Grid_Items[$index].productData[key].check_del) {
                ids.push('ids=' + $scope.Grid_Items[$index].productData[key].id);
            }
        }
        if (ids.length > 0) {
            if (confirm(msg_Info_Is_SureDelete)) {
                $http['delete'](apiProductData + '?' + ids.join('&')).success(function (data, status, headers, config) {
                    if (data.result) {
                        $scope.Detail_Init($index);
                    }
                    else {
                        toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                    }
                });
            }
        }
        else {
            alert(msg_Warn_Select_Delete_Data);
        }
    };
    $scope.SelectAllSubCheckDel = function ($index, $event) {
        for (var key in $scope.Grid_Items[$index].productData) {
            $scope.Grid_Items[$index].productData[key].check_del = $event.target.checked;
            console.log($scope.Grid_Items[$index].productData[key].check_del);
        }
    };
    $scope.Sub_Submit = function () {
        if ($scope.editTypeDetail == 1 /* insert */) {
            $http.post(apiProductData, $scope.fds).success(function (data, status, headers, config) {
                if (data.result) {
                    toaster.pop('success', js_Info_Toast_Return_Title, Info_Insert_Success);
                    $scope.editTypeDetail = 2 /* update */;
                }
                else {
                    toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                }
            });
        }
        ;
        if ($scope.editTypeDetail == 2 /* update */) {
            $http.put(apiProductData, $scope.fds).success(function (data, status, headers, config) {
                if (data.result) {
                    toaster.pop('success', js_Info_Toast_Return_Title, Info_Update_Success);
                }
                else {
                    toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                }
            });
        }
        ;
        $scope.Detail_Init($scope.findIndex($scope.fds.type_id));
    };
    $scope.Sub_Edit_Close = function () {
        $scope.Detail_Init($scope.findIndex($scope.fds.type_id));
        $scope.editTypeDetail = 0 /* none */;
        $scope.show_detail_edit = false;
        $scope.fds = {};
    };
    $scope.Sub_Open_Modify = function ($parentIndex, $index) {
        var get_id = $scope.Grid_Items[$parentIndex].productData[$index].id;
        console.log('Start get sub modify data', get_id);
        $http.get(apiProductData, { params: { id: get_id } }).success(function (data, status, headers, config) {
            if (data.result) {
                data.data = obj_prop_date(data.data);
                $scope.fds = data.data;
                $scope.show_detail_edit = true;
                $scope.editTypeDetail = 2 /* update */;
                console.info("Sub data get ok!", "Show", $scope.show_detail_edit);
            }
            else {
                console.error(data.message);
                toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
            }
        });
    };
    $scope.Sub_Open_New = function ($index) {
        var type_id = $scope.Grid_Items[$index].id;
        $scope.fds = {};
        $http.post(aj_DGetId, {}).success(function (data, status, headers, config) {
            $scope.fds = {
                id: data,
                type_id: type_id
            };
            $scope.editTypeDetail = 1 /* insert */;
            console.info("Get New Id Value:", data);
        });
        $scope.show_detail_edit = true;
    };
    $scope.findIndex = function (type_id) {
        for (var key in $scope.Grid_Items) {
            if ($scope.Grid_Items[key].id == type_id) {
                return key;
            }
        }
        ;
    };
    $http.post(aj_Init, {}).success(function (data, status, headers, config) {
        $scope.InitData = data;
    });
    $scope.Init_Query();
}]);
