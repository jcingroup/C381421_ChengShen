interface IDocCtr extends IMakeScopeBase {

}

angular
    .module('angularApp', ['commfun', 'ui.bootstrap', 'toaster'])
    .controller('ctrl', ['$scope', '$http', '$modal', 'toaster', 'gridpage',
        function ($scope: IDocCtr, $http: ng.IHttpService, $modal, toaster, gridpage) {

        }]);
