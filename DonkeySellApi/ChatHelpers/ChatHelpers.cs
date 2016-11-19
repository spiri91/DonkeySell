using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace DonkeySellApi.ChatHelpers
{
    public interface IChatHelpers
    {
        void AddOrUpdate(UserConnection userConnection);
        void Remove(string username);
        UserConnection GetUserConnection(string username);
        List<string> FindListOfUser(string user);
        List<string> FindListOfFriend(string user);
    }

    public class ChatHelpers : IChatHelpers
    {
        private static List<UserConnection> UserConnections = new List<UserConnection>();

        private ICrudOnFriends crudOnFriends;

        public ChatHelpers(ICrudOnFriends crudOnFriends)
        {
            this.crudOnFriends = crudOnFriends;
        }


        public void AddOrUpdate(UserConnection userConnection)
        {
            if (CheckIfUserConnectionIsPresent(userConnection.Username))
            {
                UserConnections.Single(x => x.Username == userConnection.Username).ConnectionId =
                    userConnection.ConnectionId;

                return;
            }

            UserConnections.Add(userConnection);
        }

        public void Remove(string username)
        {
            if (CheckIfUserConnectionIsPresent(username))
            {
                var userConnection = UserConnections.Single(x => x.Username == username);
                UserConnections.Remove(userConnection);
            }
        }

        public UserConnection GetUserConnection(string username)
        {
            if (CheckIfUserConnectionIsPresent(username))
                return UserConnections.Single(x => x.Username == username);

            return null;
        }

        private bool CheckIfUserConnectionIsPresent(string username)
        {
            return UserConnections.Any(x => x.Username == username);
        }

        public List<string> FindListOfFriend(string user)
        {
            return crudOnFriends.GetFriendsOfUser(user).Result;
        }

        public List<string> FindListOfUser(string user)
        {
            return crudOnFriends.GetUsersOfFriend(user).Result;
        }
    }
}