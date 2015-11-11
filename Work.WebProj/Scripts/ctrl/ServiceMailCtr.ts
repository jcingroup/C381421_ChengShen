interface IServiceMail extends ng.IScope {
    Mail_Submit(): void;
    fd: any[];
}
angular
    .module('angularApp', ['commfun'])
    .controller('ctrl', ['$scope', '$http', 'gridpage', 'workService',
    function ($scope: IServiceMail, $http: ng.IHttpService, gridpage, workService: services.workService) {
        $scope.Mail_Submit = function () {
            //console.info("Update Mode Start...");
            $http.post("Service/sendMail", $scope.fd)
                .success(function (data: IResultBase, status, headers, config) {
                if (data.result) {
                    alert("送信成功!");
                    console.info("Update Finish");
                } else {
                    alert(data.message);
                }
            });
        };
    }]);
