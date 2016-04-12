var myControllers = angular.module('myControllers', []);

myControllers.controller('myCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.fave = localStorage.getItem('fave');

    $scope.labels = [];
    $scope.data = [[]];
    $scope.series = ['Capacity'];

    $scope.setFave = function (team) {
        $scope.fave = team.Code;
        $scope.team = team;
        localStorage.setItem("fave", $scope.fave);
    }

    $http.get('data/team.json').then(function (data) {
        $scope.teams = data.data;
        for (i = 0; i < $scope.teams.length; i++) {
            $scope.labels.push($scope.teams[i].Code);
            $scope.data[0].push($scope.teams[i].Capacity);
        }
    });

    $http.get('data/player.json').then(function (data) {
        $scope.players = data.data;
    });

}]);

myControllers.controller('myTeamCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.fave = localStorage.getItem('fave');

    $http.get('data/team.json').then(function (data) {
        $scope.teams = data.data;
        for (i = 0; i < $scope.teams.length; i++) {
            if ($scope.fave == $scope.teams[i].Code) {
                $scope.team = $scope.teams[i];
                break;
            }
        }
    });

    $http.get('data/player.json').then(function (data) {
        $scope.players = data.data;
    });

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

}]);