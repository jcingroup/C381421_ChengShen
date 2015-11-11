interface IProductDataCtr extends IScopeM<server.ProductData> {
    editorOptions: any;
    show_detail_edit: boolean;
    editTypeDetail: IEditType;
    is_SecondData: labelValue<boolean>[];//全新/中古 分類
    q_is_SecondData: labelValue<boolean>[];//全新/中古 分類
    query_producttype: IKeyValue[];//分類
    producttype: IKeyValue[];//分類

    //用id找index
    findIndex(id: number): number;
    //搜尋分類選單
    Query_changeType(): void;
    //分類選單
    changeType(): void;
}


angular
    .module('angularApp', ['commfun', 'ui.bootstrap', 'ngCkeditor', 'toaster', 'commbox'])
    .controller('ctrl', ['$scope', '$http', '$modal', 'toaster', 'gridpage',
    function ($scope: IProductDataCtr, $http: ng.IHttpService, $modal, toaster, gridpage) {

        $scope.editorOptions = CKConfig;
        $scope.is_SecondData = commData.isSecondData;
        $scope.q_is_SecondData = commData.q_isSecondData;
        $scope.sd = { PageSize: 10, is_second:null};

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
            $scope.fd = <server.ProductData>{};
        };
        $scope.Master_Open_Modify = function ($index) {

            var get_id = $scope.Grid_Items[$index].id;
            console.log('Start get master modify data', get_id);

            $http.get(apiConnection, { params: { id: get_id } })//gb_approot +
                .success(function (data: IResultBase, status, headers, config) {

                if (data.result) {
                    data.data = obj_prop_date(data.data);
                    $scope.fd = data.data;
                    $scope.changeType();
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
            $scope.fd = <server.ProductData>{};
            $http.post(aj_MGetId, {})
                .success(function (data: number, status, headers, config) {
                $scope.fd = <server.ProductData>{
                    id: data
                };
                $scope.edit_type = IEditType.insert;
                console.info("Get New Id Value:", data);
            });
            $scope.show_master_edit = true;
        };

        $scope.findIndex = function (type_id: number) {
            for (var key in $scope.Grid_Items) {
                if ($scope.Grid_Items[key].id == type_id) {
                    return key;
                }
            };
        }

        $scope.Query_changeType = function () {
            if ($scope.sd.is_second) {
                $scope.query_producttype = $scope.InitData.options_true;
            } else {
                $scope.query_producttype = $scope.InitData.options_false;
            }
        };
        $scope.changeType = function () {
            if ($scope.fd.is_second) {
                $scope.producttype = $scope.InitData.options_true;
            } else {
                $scope.producttype = $scope.InitData.options_false;
            }
        };

        $http.post(aj_Init, {})
            .success(function (data, status, headers, config) {
            //options_equipment_category
            $scope.InitData = data;
        });


        $scope.Init_Query(); //第一次進入
    }]);
//----------------------------
