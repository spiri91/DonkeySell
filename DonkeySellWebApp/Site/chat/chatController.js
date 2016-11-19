'use strict';

app.controller('chatController', ['$scope', 'storageService', 'othersService', 'friendsService', '$mdDialog', 'toastr', '$location', chatController]);

function chatController($scope, storageService, othersService, friendsService, $mdDialog, toastr, $location) {
    $scope.usernameLike = "";
    $scope.foundUsers = [];
    $scope.onlineFriends = [];
    $scope.friends = [];

    $scope.newGeneralMessage = "";
    $scope.generalMessages = [];
    $scope.privateChatUsers = [];
    $scope.privateMessages = {};

    var originatorEv;

    // signalR initialization
    $.connection.hub.qs = { 'access_token': storageService.get('token') };
    $.connection.hub.url = 'http://localhost:57792/signalr';
    setTimeout(function () {
        $.connection.hub.start({ jsonp: true })
            .done(function () {
                $scope.chatHub.server.findOnlineFriends($scope.$parent.username);
                $scope.chatHub.server.sendOnlineNotification($scope.$parent.username);
            });
    }, 3000);
    $scope.chatHub = $.connection.chatHub;
    // finished initialization

    $scope.openMenu = function ($mdOpenMenu, ev) {
        originatorEv = ev;
        $mdOpenMenu(ev);
    };

    $scope.$on('logout',
        function () {
            $.connection.hub.stop();
        });

    $scope.searchForUsers = function () {
        if ($scope.usernameLike.length >= 4)
            othersService.usersLike($scope.usernameLike)
                .then(function (users) {
                    if (users.data)
                        $scope.foundUsers = users.data;
                }, function (error) {
                    $scope.doSomethingWithError(error);
                });
    };

    $scope.addToFriends = function (user) {
        if (user !== $scope.$parent.username && ($.inArray(user, $scope.friends) === -1)) {
            let token = storageService.get('token');
            friendsService.postFriend($scope.$parent.username, user, token)
                .then(function () {
                    $scope.chatHub.server.checkIfUserIsOnline(user);
                    $scope.friends.push(user);
                    toastr.success('Added!');
                }, function (error) {
                    $scope.doSomethingWithError(error);
                });
        } else {
            toastr.error('Allready added!');
        }
    };

    $scope.openConversation = function (user) {
        if ($scope.privateChatUsers.indexOf(user) === -1) {
            $scope.privateChatUsers.push(user);
            $scope.privateMessages[user] = [];
        }
    }

    $scope.init = function () {
        $scope.getFriends();
    }

    $scope.goToUserProfile = function (user) {
        $location.url('/user/' + user);
    }

    $scope.getFriends = function () {
        let token = storageService.get('token');
        friendsService.getFriends($scope.$parent.username, token)
            .then(function (friends) {
                if (friends.data)
                    $scope.friends = friends.data;
            }, function (error) {
                $scope.doSomethingWithError(error);
            });
    }

    $scope.removeFriend = function (user) {
        var confirm = $mdDialog.confirm()
         .title('Are you sure you want to delete him?! :(')
         .ok("Yes, i'm sure!")
         .cancel('No!');

        $mdDialog.show(confirm).then(function () {
            let token = storageService.get('token');
            friendsService.deleteFriend($scope.$parent.username, user, token)
                .then(function () {
                    let index = $scope.onlineFriends.indexOf(user);
                    $scope.onlineFriends.splice(index, 1);
                    index = $scope.friends.indexOf(user);
                    $scope.friends.splice(index, 1);
                    toastr.success('Removed!');
                }, function (error) {
                    $scope.doSomethingWithError(error);
                });
        });
    }

    $scope.sendPrivateMessage = function (user) {
        let id = 'private' + user;
        let message = document.getElementById(id).value;

        if (!message)
            return;

        let privateMessage = new PrivateMessage(Date.now(), $scope.$parent.username, message);
        $scope.$apply(function () {
            $scope.privateMessages[user].push(privateMessage);
        });
        $scope.chatHub.server.sendPrivateMessage(message, user);
        document.getElementById(id).value = "";

        let divId = 'chatWith' + user;
        $scope.scrollIntoView(divId);
    }

    $scope.closeConversation = function (user) {
        let index = $scope.privateChatUsers.indexOf(user);
        $scope.privateChatUsers.splice(index, 1);
    }

    $scope.sendGeneralMessage = function () {
        if (!$scope.newGeneralMessage)
            return;

        $scope.chatHub.server.sendMessageToAll($scope.newGeneralMessage);
        $scope.newGeneralMessage = "";
    };

    $scope.chatHub.client.addGeneralMessage = function (username, message) {
        let generalMessage = new GeneralMessage(username, message);
        $scope.$apply(function () {
            $scope.generalMessages.push(generalMessage);
        });

        $scope.scrollIntoView("chatDiv");
    }

    $scope.scrollIntoView = function (id) {
        let objDiv = document.getElementById(id);
        objDiv.scrollTop = objDiv.scrollHeight;
    }

    $scope.chatHub.client.addPrivateMessage = function (user, message) {
        $scope.$apply(function () {
            $scope.openConversation(user);
            let privateMessage = new PrivateMessage(Date.now(), user, message);
            $scope.privateMessages[user].push(privateMessage);
        });

        let divId = 'chatWith' + user;
        $scope.scrollIntoView(divId);
    }

    $scope.chatHub.client.addOnlineUser = function (user) {
        $scope.$apply(function () {
            $scope.onlineFriends.push(user);
        });
    };

    $scope.chatHub.client.addStatusNottification = function (user, status) {
        toastr.warning(user + " is now " + status);
        
            let index = $scope.onlineFriends.indexOf(user);
            if (status === 'Online') {
                if (index === -1)
                    $scope.$apply(function() {
                        $scope.onlineFriends.push(user);
                    });
                if ($scope.privateChatUsers.indexOf(user) !== -1) {
                    $scope.chatHub.client.addPrivateMessage(user, " is online!");
                }

            } else {
                if (index >= 0)
                    $scope.$apply(function() {
                        $scope.onlineFriends.splice(index, 1);
                    });
                if ($scope.privateChatUsers.indexOf(user) !== -1)
                    $scope.chatHub.client.addPrivateMessage(user, " is offline!");
            }
    }

    $scope.doSomethingWithError = function (error) {
        console.log(error);
    }

    $scope.init();
};

function GeneralMessage(sender, value) {
    this.sender = sender;
    this.value = value;
}

function PrivateMessage(id, sender, value) {
    this.id = id;
    this.sender = sender;
    this.value = value;
}