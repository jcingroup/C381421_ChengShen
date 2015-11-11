angular
    .module('angularApp', ['commfun', 'ui.bootstrap', 'toaster'])
    .controller('ctrl', ['$scope', '$http', '$modal', 'toaster', 'gridpage',
    function ($scope, $http, $modal, toaster, gridpage) {
        $scope.Init_Query = function () {
            $http.post(aj_Init, {})
                .success(function (data, status, headers, config) {
                $scope.fd = {
                    receiveMails: data.receiveMails,
                };
                console.info("getdata Finish");
            });
        };
        $scope.Master_Submit = function () {
            console.info("Update Mode Start...");
            $http.post(aj_MUpdata, $scope.fd)
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
        };
        $scope.Init_Query();
    }]);
