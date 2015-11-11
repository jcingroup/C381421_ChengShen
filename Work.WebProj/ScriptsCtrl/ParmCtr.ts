interface IParmCtr extends IMakeScopeBase {

}

angular
    .module('angularApp', ['commfun', 'ui.bootstrap', 'toaster'])
    .controller('ctrl', ['$scope', '$http', '$modal', 'toaster', 'gridpage',
    function ($scope: IParmCtr, $http: ng.IHttpService, $modal, toaster, gridpage) {

        $scope.Init_Query = function () {
            $http.post(aj_Init, {})
                .success(function (data: server.Parm, status, headers, config) {
                $scope.fd = <server.Parm>{
                    //layer: data.layer,
                    //surfacehandle:data.surfacehandle,
                    receiveMails: data.receiveMails,
                    //BccMails: data.BccMails
                };
                console.info("getdata Finish");
            });
        }

        $scope.Master_Submit = function () {
            console.info("Update Mode Start...");
            $http.post(aj_MUpdata, $scope.fd)
                .success(function (data: IResultBase, status, headers, config) {
                if (data.result) {
                    toaster.pop('success', js_Info_Toast_Return_Title, Info_Update_Success);
                    $scope.Init_Query();
                    console.info("Update Finish");
                } else {
                    toaster.pop('error', msg_Err_System_Failure, data.message, 5000);
                }
            });
        };

        $scope.Init_Query(); //第一次進入
    }]);
