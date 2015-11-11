angular.module('angularApp', ['commfun']).controller('ctrl', ['$scope', '$http', 'gridpage', 'workService', function ($scope, $http, gridpage, workService) {
    $scope.Mail_Submit = function () {
        $http.post("Service/sendMail", $scope.fd).success(function (data, status, headers, config) {
            if (data.result) {
                alert("送信成功!");
                console.info("Update Finish");
            }
            else {
                alert(data.message);
            }
        });
    };
}]);
