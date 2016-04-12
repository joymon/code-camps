var myControllers = angular.module('myControllers', []);

myControllers.controller('myCtrl', function ($scope, $http, dataService) {

    $scope.view = 'teams';
    $scope.fave = localStorage.getItem('fave');

    $scope.labels = [];
    $scope.data = [[]];
    $scope.series = ['Capacity'];

    $scope.setView = function (view) {
        $scope.view = view;
    }

    $scope.setFave = function (team) {
        $scope.fave = team.Code;
        $scope.team = team;
        localStorage.setItem("fave", $scope.fave);
    }

    dataService.getTeams().then(function (result) {
        $scope.teams = result;
        for (i = 0; i < $scope.teams.length; i++) {
            $scope.labels.push($scope.teams[i].Code);
            $scope.data[0].push($scope.teams[i].Capacity);
        }
        console.log($scope.labels);
    });
    dataService.getPlayers().then(function (result) { $scope.players = result; });

    navigator.geolocation.watchPosition(showPosition);

    function showPosition(position) {
        $scope.latitude = position.coords.latitude;
        $scope.longitude = position.coords.longitude;
    }

    $scope.distance = function (team, unit) {

        var lat1 = $scope.latitude;
        var lon1 = $scope.longitude;
        var lat2 = team.Lat;
        var lon2 = team.Long;

        var radlat1 = Math.PI * lat1 / 180
        var radlat2 = Math.PI * lat2 / 180
        var theta = lon1 - lon2
        var radtheta = Math.PI * theta / 180
        var dist = Math.sin(radlat1) * Math.sin(radlat2) + Math.cos(radlat1) * Math.cos(radlat2) * Math.cos(radtheta);
        dist = Math.acos(dist)
        dist = dist * 180 / Math.PI
        dist = dist * 60 * 1.1515
        if (unit == "K") { dist = dist * 1.609344 }
        if (unit == "N") { dist = dist * 0.8684 }
        return dist
    }

    $scope.infield = function (player) {
        if (player.Pos == 'SS') return true;
        else if (player.Pos.indexOf('B') > -1) return true;
        else return false;
    };

}).directive('myteam', function () {
    return {
        restrict: 'E',
        templateUrl: '8-myteam.html'
    };
}).service('dataService', ['$http', '$q', function ($http, $q) {
    this.getTeams = function () {
        var dfd = $q.defer();
        $http.get('data/team.json').success(function (data) {
            dfd.resolve(data);
        }).error(function (data) {
            dfd.reject(data);
        });
        return dfd.promise;
    }

    this.getPlayers = function () {
        var dfd = $q.defer();
        $http.get('data/player.json').success(function (data) {
            dfd.resolve(data);
        }).error(function (data) {
            dfd.reject(data);
        });
        return dfd.promise;
    }
}]);
