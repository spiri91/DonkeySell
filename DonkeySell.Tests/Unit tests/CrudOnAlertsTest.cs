using DonkeySellApi.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.Shared;
using DonkeySellApi.Models.ViewModels;
using Ninject;
using NUnit.Framework;

namespace DonkeySell.Tests.Unit_tests
{
    class CrudOnAlertsTest
    {
        private ICrudOnAlerts crudOnAlerts;
        private ICrudOnUsers crudOnUsers;
        private DonkeySellUser user;
        private Alert alert;
        private string productName = "testProduct";

        [SetUp]
        public void Initialize()
        {
            TestInitialiser.Initialise();
            crudOnAlerts = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnAlerts>();
            crudOnUsers = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnUsers>();
            var viewUser = TestInitialiser.CreateUser();
            user = crudOnUsers.CreateOrUpdateUser(viewUser).Result;
            var viewAlert = new ViewAlert() {ProductName = productName, UserId = user.UserId};
            alert = Mapper.Map<Alert>(viewAlert);
        }

        [TearDown]
        public void TearDown()
        {
            crudOnUsers.DeleteUser(user.UserName);
        }

        [Test]
        public void ShouldAddAlert()
        {
            var newAlert = crudOnAlerts.AddAlert(alert).Result;
            Assert.NotNull(newAlert);
            crudOnAlerts.DeleteAlert(newAlert.Id).Wait();
        }

        [Test]
        public void ShouldGetAlerts()
        {
            crudOnAlerts.AddAlert(alert).Wait();
            var alerts = crudOnAlerts.GetAlerts(user.UserId).Result;
            Assert.True(alerts.Count > 0);
            crudOnAlerts.DeleteAlert(alert.Id).Wait();
        }

        [Test]
        public void ShouldRemoveAlert()
        {
            crudOnAlerts.AddAlert(alert).Wait();
            var id = crudOnAlerts.DeleteAlert(alert.Id).Result;
            Assert.NotNull(id);
        }

        [Test]
        public void ShouldGetEmailsForAlert()
        {
            crudOnAlerts.AddAlert(alert).Wait();
            var emails = crudOnAlerts.GetUsersWithAlertProduct(productName).Result;
            Assert.True(emails.Count > 0);
            crudOnAlerts.DeleteAlert(alert.Id).Wait();
        }
    }
}
