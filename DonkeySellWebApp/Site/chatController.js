app.controller('chatController', ['$scope','storageService', chatController]);

function chatController($scope, storageService) {
    $scope.messages = [];
    $scope.newMessage = "";

   
    $.connection.hub.qs = { 'access_token': storageService.get('token')};
    $.connection.hub.url = 'http://localhost:57792/signalr';
    $scope.chatHub = $.connection.chatHub;
    $.connection.hub.start({ jsonp: true });

    $scope.chatHub.client.addMessage = function(username, message) {
        let fullMessage = username + " says " + message + " "+Date.now();
        $scope.messages.push(fullMessage);
    }

    $scope.send = function() {
        $scope.chatHub.server.sendMessage($scope.$parent.username, $scope.newMessage);
    };

    $scope.sendPrivate  = function (){
        $scope.chatHub.server.sendPrivateMessage($scope.$parent.username, $scope.newMessage, "Ionut");
    }
};