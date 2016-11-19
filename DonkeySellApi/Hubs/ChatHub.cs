using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using DonkeySellApi.ChatHelpers;
using DonkeySellApi.Models.DatabaseModels;
using Microsoft.AspNet.SignalR;
using Ninject;

namespace DonkeySellApi.Hubs
{
    public enum Status
    {
        Online, 
        Offline,
    }

    [Authorize]
    public class ChatHub : Hub
    {
        private IChatHelpers chatHelpers { get; set; }

        public ChatHub(IChatHelpers chatHelpers)
        {
            this.chatHelpers = chatHelpers;
        }

        public override Task OnConnected()
        {
            var username = Context.User.Identity.Name;
            var connectionId = Context.ConnectionId;
            var userConnection = new UserConnection() {ConnectionId = connectionId, Username = username};
            chatHelpers.AddOrUpdate(userConnection);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var username = Context.User.Identity.Name;
            chatHelpers.Remove(username);
            SendOfflineNotification(username);

            return base.OnDisconnected(stopCalled);
        }

        public void Hello()
        {
            Clients.All.hello();
        }
      
        public void SendMessageToAll(string message)
        {
            var sender = Context.User.Identity.Name;
            Clients.All.addGeneralMessage(sender, message);
        }

        public void SendPrivateMessage(string message, string to)
        {
            var sender = Context.User.Identity.Name;
            var toUser = chatHelpers.GetUserConnection(to);
            
            if (toUser != null && sender != null)
            {
                Clients.Client(toUser.ConnectionId).addPrivateMessage(sender, message);
            }
        }

        public void FindOnlineFriends(string user)
        {
            foreach (var friend in chatHelpers.FindListOfFriend(user))
            {
                if(chatHelpers.GetUserConnection(friend)!=null)
                    Clients.Caller.addOnlineUser(friend);
            }
        }

        public void CheckIfUserIsOnline(string user)
        {
            if (chatHelpers.GetUserConnection(user) != null)
                Clients.Caller.addOnlineUser(user);
        }

        public void SendOnlineNotification(string user)
        {
            SendNotification(user, Status.Online);
        }

        private void SendNotification(string user, Status status)
        {
            var userFriends = chatHelpers.FindListOfUser(user);
            foreach (string  userFriend in userFriends)
            {
                if (chatHelpers.GetUserConnection(userFriend) != null)
                {
                    var connectionId = chatHelpers.GetUserConnection(userFriend).ConnectionId;
                    Clients.Client(connectionId).addStatusNottification(user, status.ToString());
                }
            }
        }

        private void SendOfflineNotification(string user)
        {
            SendNotification(user, Status.Offline);
        }
    }
}