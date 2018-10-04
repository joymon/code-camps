var myApp = angular.module('myApp', [
    'datatables',
    'chart.js',
    'picardy.fontawesome',
    'myControllers',
    'ngRoute'
]);

myApp.config(['$routeProvider',
function ($routeProvider) {
    $routeProvider.
    when('/myteam', {
        templateUrl: '9-myteam.html',
        controller: 'myTeamCtrl'
    }).
    when('/teams', {
        templateUrl: '9-teams.html',
        controller: 'myCtrl'
    }).
    when('/players', {
        templateUrl: '9-players.html',
        controller: 'myCtrl'
    }).
    when('/capacity', {
        templateUrl: '9-capacity.html',
        controller: 'myCtrl'
    }).
    when('/pick', {
        templateUrl: '9-pick.html',
        controller: 'myCtrl'
    }).
    otherwise({
        redirectTo: '/teams'
    });
}]);