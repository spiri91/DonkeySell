using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Workers;
using Ninject;
using NUnit.Framework;

namespace DonkeySell.Tests.Unit_tests
{
    class CrudOnFriendsTests
    {
        private ICrudOnFriends crudOnFriends;
        private ICrudOnUsers crudOnUsers;
        private string firstUsername = "user1";
        private string secondUsername = "user2";

        [SetUp]
        public void Initialize()
        {
            TestInitialiser.Initialise();
            crudOnFriends = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnFriends>();
            crudOnUsers = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnUsers>();
            AddTwoUsers();
        }

        [TearDown]
        public void TearDown()
        {
            DeleteThoseTwoUsers();
        }

        private void DeleteThoseTwoUsers()
        {
            crudOnUsers.DeleteUser(firstUsername).Wait();
            crudOnUsers.DeleteUser(secondUsername).Wait();
        }

        private void AddTwoUsers()
        {
            var user1 = TestInitialiser.CreateUser();
            user1.UserName = firstUsername;
            user1.Email = "Bla@bla.com";
            var user2 = TestInitialiser.CreateUser();
            user2.UserName = secondUsername;
            user2.Email = "bla1@bla1.com";
            crudOnUsers.CreateOrUpdateUser(user1).Wait();
            crudOnUsers.CreateOrUpdateUser(user2).Wait();
        }

        [Test]
        public void ShouldGetFriends()
        {
            crudOnFriends.AddUserFriend(firstUsername, secondUsername).Wait();
            var friends = crudOnFriends.GetFriendsOfUser(firstUsername).Result;
            Assert.AreEqual(friends[0], secondUsername);
            crudOnFriends.DeleteFriendFromUser(firstUsername, secondUsername).Wait();
           
        }

        [Test]
        public void ShouldAddFriend()
        {
            var newFriend = crudOnFriends.AddUserFriend(firstUsername, secondUsername).Result;
            Assert.AreEqual(newFriend.Username, firstUsername);
            Assert.AreEqual(newFriend.FriendUser, secondUsername);
            crudOnFriends.DeleteFriendFromUser(firstUsername, secondUsername).Wait();
        }

        [Test]
        public void ShouldRemoveFriend()
        {
            var newFriend = crudOnFriends.AddUserFriend(firstUsername, secondUsername).Result;
            var deletedId = crudOnFriends.DeleteFriendFromUser(firstUsername, secondUsername).Result;
            Assert.AreEqual(newFriend.Id, deletedId);
        }
    }
}
