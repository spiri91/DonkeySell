using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;

public interface ICrudOnFriends
{
    Task<List<string>> GetFriendsOfUser(string username);

    Task<Friend> AddUserFriend(string username, string friend);

    Task<int> DeleteFriendFromUser(string username, string friend);

    Task DeleteUserFromListOfFriends(string username);

    Task<List<string>> GetUsersOfFriend(string friend);
}

namespace DonkeySellApi.Workers
{
    public class CrudOnFriends : ICrudOnFriends, IDisposable
    {
        private DonkeySellContext context;

        public CrudOnFriends(DonkeySellContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> GetFriendsOfUser(string username)
        {
            var friends = context.Friends.Where(x => x.Username == username).Select(x => x.FriendUser).ToList();

            return friends;
        }

        public async Task<Friend> AddUserFriend(string username, string friend)
        {
            if (!context.Users.Any(x => x.UserName == username || x.UserName == friend))
                throw new ObjectNotFoundException();


            if (context.Friends.Any(x => x.Username == username && x.FriendUser == friend))
                throw new FormatException();

            var newFriend = context.Friends.Add(new Friend() { Username = username, FriendUser = friend });
            await context.SaveChangesAsync();

            return newFriend;
        }

        public async Task<int> DeleteFriendFromUser(string username, string friend)
        {
            if (!context.Users.Any(x => x.UserName == username || x.UserName == friend))
                throw new ObjectNotFoundException();

            if (!context.Friends.Any(x => x.Username == username && x.FriendUser == friend))
                throw new FormatException();

            var friendToDelete = context.Friends.Single(x => x.Username == username && x.FriendUser == friend);
            var id = context.Friends.Remove(friendToDelete).Id;
            await context.SaveChangesAsync();

            return id;
        }

        public async Task DeleteUserFromListOfFriends(string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var listOfFriends = context.Friends.Where(x => x.Username == username || x.FriendUser == username).ToList();
            context.Friends.RemoveRange(listOfFriends);
            await context.SaveChangesAsync();
        }

        public async Task<List<string>> GetUsersOfFriend(string friend)
        {
            if (!context.Users.Any(x => x.UserName == friend))
                throw new ObjectNotFoundException();

            var listOfUsers = context.Friends.Where(x => x.FriendUser == friend).Select(x => x.Username).ToList();
            return listOfUsers;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}