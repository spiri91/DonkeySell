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
            var donkeySellUser = crudOnUsers.CreateOrUpdateUser(this.user).Result;
            Assert.IsNotNull(donkeySellUser);
            crudOnUsers.DeleteUser(user.UserName).Wait();
        }

        [Test]
        public void ShouldGetUser()
        {
            crudOnUsers.CreateOrUpdateUser(user).Wait();
            var viewUser = crudOnUsers.GetUser(user.UserName).Result;
            Assert.NotNull(viewUser);
        }

        [Test]
        public void ShouldDeleteUser()
        {
            var dbUser = crudOnUsers.CreateOrUpdateUser(user).Result;
            var id = crudOnUsers.DeleteUser(dbUser.UserName).Result;
            Assert.IsNotNull(id);
        }

        [Test]
        public void ShouldUpdateUser()
        {
            var newEmail = "newTest@test.com";
            var newFacebook = "newFacebookAddress";
            var dbUser = crudOnUsers.CreateOrUpdateUser(user).Result;
            dbUser.Email = newEmail;
            dbUser.Facebook = newFacebook;
            dbUser.Password = "Super91!";
            var newDbUser = crudOnUsers.CreateOrUpdateUser(dbUser).Result;
            Assert.AreEqual(newDbUser.Email, newEmail);
            Assert.AreEqual(newDbUser.Facebook, newFacebook);
            crudOnUsers.DeleteUser(user.UserName).Wait();
        }
    }
}
