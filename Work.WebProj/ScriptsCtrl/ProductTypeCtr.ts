interface IProductTypeCtr extends IScopeM<server.ProductType> {
    show_detail_edit: boolean;
    editTypeDetail: IEditType;
    is_SecondData: labelValue<boolean>[];//全新/中古 分類

    //用id找index
    findIndex(id: number): number;
}

angular
    .module('angularApp', ['commfun', 'ui.bootstrap', 'toaster'])
    .controller('ctrl', ['$scope', '$http', '$modal', 'toaster', 'gridpage',
    function ($scope: IProductTypeCtr, $http: ng.IHttpService, $modal, toaster, gridpage) {

        $scope.is_SecondData = commData.q_isSecondData;
        $scope.sd = { PageSize: 10 };
        $scope.show_master_edit = false;
        $scope.show_detail_edit = false;
        $scope.edit_type = IEditType.none; //ref 2
        var timer = false; //ref 3

        $scope.grid_new_show = true;
        $scope.grid_del_show = false;
        $scope.grid_nav_show = true;

        $scope.check_del_value = false;



        $scope.JumpPage = function (page: number) {
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
                    //ids.push($scope.Grid_Items[key].id);
                    ids.push('ids=' + $scope.Grid_Items[key].id);
                }
            }

            if (ids.length > 0) {
                if (confirm(msg_Info_Is_SureDelete)) {
                    $http['delete'](apiConnection + '?' + ids.join('&')) //IE8 問題須採用此方式
                        .success(function (data: IResultBase, status, headers, config) {
                        if (data.result) {
                            $scope.Init_Query();
                        } else {
                            toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                        }
                    });
                }
            } else {
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
                $http.post(apiConnection, $scope.fd)//gb_approot +
                    .success(function (data: IResultBase, status, headers, config) {
                    if (data.result) {
                        $scope.edit_type = IEditType.update;
                        toaster.pop('success', js_Info_Toast_Return_Title, Info_Insert_Success);
                        $scope.Init_Query();
                        console.info("Insert Finish", $scope.edit_type);
                    } else {
                        toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                    }
                });
            }

            if ($scope.edit_type == IEditType.update) {
                console.info("Update Mode Start...");
                $http.put(apiConnection, $scope.fd)//gb_approot +
                    .success(function (data: IResultBase, status, headers, config) {
                    if (data.result) {
                        toaster.pop('success', js_Info_Toast_Return_Title, Info_Update_Success);
                        $scope.Init_Query();
                        console.info("Update Finish");
                    } else {
                        toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                    }
                });
            }
        };
        $scope.Master_Edit_Close = function () {
            $scope.edit_type = IEditType.none;
            $scope.show_master_edit = false;
            $scope.fd = <server.ProductType>{};
        };
        $scope.Master_Open_Modify = function ($index) {

            var get_id = $scope.Grid_Items[$index].id;
            console.log('Start get master modify data', get_id);

            $http.get(apiConnection, { params: { id: get_id } })//gb_approot +
                .success(function (data: IResultBase, status, headers, config) {

                if (data.result) {
                    data.data = obj_prop_date(data.data);
                    $scope.fd = data.data;
                    $scope.show_master_edit = true;
                    $scope.edit_type = IEditType.update;
                    console.info("Master data get ok!", "Show", $scope.show_master_edit);
                } else {
                    console.error(data.message);
                    toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                }
            });
        };
        $scope.Master_Open_New = function () {
            $scope.fd = <server.ProductType>{};
            $http.post(aj_MGetId, {})
                .success(function (data: number, status, headers, config) {
                $scope.fd = <server.ProductType>{
                    id: data
                };
                $scope.edit_type = IEditType.insert;
                console.info("Get New Id Value:", data);
            });
            $scope.show_master_edit = true;
        };

        ////sub detail---start
        //$scope.Detail_Init = function ($index: number) {
        //    var get_id = $scope.Grid_Items[$index].id;
        //    $http.get(apiProductData, { params: { type_id: get_id } })
        //        .success(function (data: IResultData<server.ProductData[]>, status, headers, config) {
        //        if (data.result) {
        //            $scope.Grid_Items[$index].productData = data.data;
        //        } else {
        //            console.error(data.message);
        //            toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
        //        };
        //    });
        //}
        //$scope.Sub_Detail_Delete = function ($index: number) {
        //    var ids = [];
        //    for (var key in $scope.Grid_Items[$index].productData) {
        //        if ($scope.Grid_Items[$index].productData[key].check_del) {
        //            ids.push('ids=' + $scope.Grid_Items[$index].productData[key].id);
        //        }
        //    }

        //    if (ids.length > 0) {
        //        if (confirm(msg_Info_Is_SureDelete)) {
        //            $http['delete'](apiProductData + '?' + ids.join('&')) //IE8 問題須採用此方式
        //                .success(function (data: IResultBase, status, headers, config) {
        //                if (data.result) {
        //                    $scope.Detail_Init($index);
        //                } else {
        //                    toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
        //                }
        //            });
        //        }
        //    } else {
        //        alert(msg_Warn_Select_Delete_Data);
        //    }
        //}
        //$scope.SelectAllSubCheckDel = function ($index: number, $event: any) {
        //    for (var key in $scope.Grid_Items[$index].productData) {
        //        $scope.Grid_Items[$index].productData[key].check_del = $event.target.checked;
        //        console.log($scope.Grid_Items[$index].productData[key].check_del);
        //    }
        //}
        //$scope.Sub_Submit = function () {
        //    //增新模式
        //    if ($scope.editTypeDetail == IEditType.insert) {
        //        $http.post(apiProductData, $scope.fds)
        //            .success(function (data: IResultBase, status, headers, config) {
        //            if (data.result) {
        //                toaster.pop('success', js_Info_Toast_Return_Title, Info_Insert_Success);
        //                $scope.editTypeDetail = IEditType.update;
        //            } else {
        //                toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
        //            }
        //        });
        //    };

        //    //修改模式
        //    if ($scope.editTypeDetail == IEditType.update) {

        //        $http.put(apiProductData, $scope.fds)
        //            .success(function (data: IResultBase, status, headers, config) {
        //            if (data.result) {
        //                toaster.pop('success', js_Info_Toast_Return_Title, Info_Update_Success);
        //            } else {
        //                toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
        //            }
        //        });
        //    };
        //    $scope.Detail_Init($scope.findIndex($scope.fds.type_id));
        //}
        //$scope.Sub_Edit_Close = function () {
        //    $scope.Detail_Init($scope.findIndex($scope.fds.type_id));
        //    $scope.editTypeDetail = IEditType.none;
        //    $scope.show_detail_edit = false;
        //    $scope.fds = <server.ProductData>{};
        //}
        //$scope.Sub_Open_Modify = function ($parentIndex: number, $index: number) {
        //    var get_id = $scope.Grid_Items[$parentIndex].productData[$index].id;
        //    console.log('Start get sub modify data', get_id);

        //    $http.get(apiProductData, { params: { id: get_id } })
        //        .success(function (data: IResultBase, status, headers, config) {

        //        if (data.result) {
        //            data.data = obj_prop_date(data.data);
        //            $scope.fds = data.data;
        //            $scope.show_detail_edit = true;
        //            $scope.editTypeDetail = IEditType.update;
        //            console.info("Sub data get ok!", "Show", $scope.show_detail_edit);
        //        } else {
        //            console.error(data.message);
        //            toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
        //        }
        //    });
        //}
        //$scope.Sub_Open_New = function ($index: number) {
        //    var type_id: number = $scope.Grid_Items[$index].id;
        //    $scope.fds = <server.ProductData>{};
        //    $http.post(aj_DGetId, {})
        //        .success(function (data: number, status, headers, config) {
        //        $scope.fds = <server.ProductData>{
        //            id: data,
        //            type_id: type_id
        //        };
        //        $scope.editTypeDetail = IEditType.insert;
        //        console.info("Get New Id Value:", data);
        //    });
        //    $scope.show_detail_edit = true;
        //}
        ////sub detail---end

        $scope.findIndex = function (type_id: number) {
            for (var key in $scope.Grid_Items) {
                if ($scope.Grid_Items[key].id == type_id) {
                    return key;
                }
            };
        }
        $http.post(aj_Init, {})
            .success(function (data, status, headers, config) {
            //options_equipment_category
            $scope.InitData = data;
        });

        $scope.Init_Query(); //第一次進入
    }]);
//----------------------------
