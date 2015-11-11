angular
    .module('angularApp', ['commfun', 'ui.bootstrap', 'toaster'])
    .controller('ctrl', ['$scope', '$http', '$modal', 'toaster', 'gridpage',
    function ($scope, $http, $modal, toaster, gridpage) {
        $scope.is_SecondData = commData.q_isSecondData;
        $scope.sd = { PageSize: 10 };
        $scope.show_master_edit = false;
        $scope.show_detail_edit = false;
        $scope.edit_type = IEditType.none;
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
                    $http['delete'](apiConnection + '?' + ids.join('&'))
                        .success(function (data, status, headers, config) {
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
            if ($scope.edit_type == IEditType.insert) {
                console.info("Insert Mode Start...");
                $http.post(apiConnection, $scope.fd)
                    .success(function (data, status, headers, config) {
                    if (data.result) {
                        $scope.edit_type = IEditType.update;
                        toaster.pop('success', js_Info_Toast_Return_Title, Info_Insert_Success);
                        $scope.Init_Query();
                        console.info("Insert Finish", $scope.edit_type);
                    }
                    else {
                        toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                    }
                });
            }
            if ($scope.edit_type == IEditType.update) {
                console.info("Update Mode Start...");
                $http.put(apiConnection, $scope.fd)
                    .success(function (data, status, headers, config) {
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
            $scope.edit_type = IEditType.none;
            $scope.show_master_edit = false;
            $scope.fd = {};
        };
        $scope.Master_Open_Modify = function ($index) {
            var get_id = $scope.Grid_Items[$index].id;
            console.log('Start get master modify data', get_id);
            $http.get(apiConnection, { params: { id: get_id } })
                .success(function (data, status, headers, config) {
                if (data.result) {
                    data.data = obj_prop_date(data.data);
                    $scope.fd = data.data;
                    $scope.show_master_edit = true;
                    $scope.edit_type = IEditType.update;
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
            $http.post(aj_MGetId, {})
                .success(function (data, status, headers, config) {
                $scope.fd = {
                    id: data
                };
                $scope.edit_type = IEditType.insert;
                console.info("Get New Id Value:", data);
            });
            $scope.show_master_edit = true;
        };
        $scope.findIndex = function (type_id) {
            for (var key in $scope.Grid_Items) {
                if ($scope.Grid_Items[key].id == type_id) {
                    return key;
                }
            }
            ;
        };
        $http.post(aj_Init, {})
            .success(function (data, status, headers, config) {
            $scope.InitData = data;
        });
        $scope.Init_Query();
    }]);
