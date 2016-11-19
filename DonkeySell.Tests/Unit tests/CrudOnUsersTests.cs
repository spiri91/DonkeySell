using AutoMapper;
using DonkeySellApi.Models.ViewModels;
using DonkeySellApi.Workers;
using Ninject;
using NUnit.Framework;

namespace DonkeySell.Tests.Unit_tests
{
    [TestFixture]
    public class CrudOnUsersTests
    {
        private ViewUser user;
        private ICrudOnUsers crudOnUsers;
       
        [SetUp]
        public void Initialize()
        {
            TestInitialiser.Initialise();
            crudOnUsers = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnUsers>();
            user = TestInitialiser.CreateUser();
        }

        [Test]
        public void ShouldAddUser()
        {
            user.UserName = "ionut95";
            user.Email = "t@t95.com";
            var donkeySellUser = crudOnUsers.CreateOrUpdateUser(user).Result;
            Assert.IsNotNull(donkeySellUser);
            crudOnUsers.DeleteUser(user.UserName).Wait();
        }

        [Test]
        public void ShouldGetUser()
        {
            user.UserName = "ionut99";
            user.Email = "t@t99.com";
            crudOnUsers.CreateOrUpdateUser(user).Wait();
            var viewUser = crudOnUsers.GetUser(user.UserName).Result;
            Assert.NotNull(viewUser);
            crudOnUsers.DeleteUser(user.UserName).Wait();
        }

        [Test]
        public void ShouldDeleteUser()
        {
            user.UserName = "ionut98";
            user.Email = "t@t98.com";
            var dbUser = crudOnUsers.CreateOrUpdateUser(user).Result;
            var id = crudOnUsers.DeleteUser(dbUser.UserName).Result;
            Assert.IsNotNull(id);
        }

        [Test]
        public void ShouldUpdateUser()
        {
            user.UserName = "ionut97";
            user.Email = "t@t97.com";
            var newEmail = "newTest@test.com";
            var newFacebook = "newFacebookAddress";
            var dbUser = crudOnUsers.CreateOrUpdateUser(user).Result;
            var viewUSer = Mapper.Map<ViewUser>(dbUser);
            viewUSer.Email = newEmail;
            viewUSer.Facebook = newFacebook;
            viewUSer.Password = "Super9108";
            var newDbUser = crudOnUsers.CreateOrUpdateUser(viewUSer).Result;
            Assert.AreEqual(newDbUser.Email, newEmail);
            Assert.AreEqual(newDbUser.Facebook, newFacebook);
            crudOnUsers.DeleteUser(user.UserName).Wait();
        }

        [Test]
        public void ShouldChangePassword()
        {
            user.UserName = "ionut96";
            user.Email = "t@t96.com";
            var newPassword = "Super9102";
            var createdUser = crudOnUsers.CreateOrUpdateUser(user).Result;
            crudOnUsers.ChangePassword(createdUser.UserName, user.Password, newPassword).Wait();
            Assert.IsTrue(true);
            crudOnUsers.DeleteUser(user.UserName).Wait();
        }
    }
}
