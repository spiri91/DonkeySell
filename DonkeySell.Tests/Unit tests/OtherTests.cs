using System.Linq;
using DonkeySellApi.Extra;
using DonkeySellApi.Workers;
using Ninject;
using NUnit.Framework;

namespace DonkeySell.Tests.Unit_tests
{
    [TestFixture]
    public class OtherTests
    {
        private IGetCitiesAndCategories getCitiesAndCategories;
        private ICrudOnProducts crudOnProducts;
        private ICrudOnAlerts crudOnAlerts;
        private IMailSender mailSender;

        [SetUp]
        public void Initialize()
        {
            TestInitialiser.Initialise();
            this.getCitiesAndCategories = TestInitialiser.ninjectKernel.kernel.Get<IGetCitiesAndCategories>();
            this.crudOnProducts = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnProducts>();
            this.crudOnAlerts = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnAlerts>();
            this.mailSender = TestInitialiser.ninjectKernel.kernel.Get<IMailSender>();
        }


        [Test]
        [Ignore("do not fill the db every time")]
        public void ShouldSeedDatabase()
        {
            DonkeySellDatabaseFunctions databaseFunctions = new DonkeySellDatabaseFunctions();
            var seedSuccessful = databaseFunctions.Seed();
            Assert.IsTrue(seedSuccessful);
        }

        [Test]
        public void ShouldRetrieveCategories()
        {
            var categories = getCitiesAndCategories.GetCategories();
            Assert.NotNull(categories);
        }

        [Test]
        public void ShouldRetrieveCaties()
        {
            var cities = getCitiesAndCategories.GetCities();
            Assert.NotNull(cities);
        }

        [Test]
        public void ShouldSentProductAlert()
        {
            ProductEmailNotifications productEmailNotifications = new ProductEmailNotifications(crudOnAlerts, mailSender);
            var product = TestInitialiser.CreateProduct();
            product.Title = "ceva bun tare";
            var addedProduct = crudOnProducts.AddOrUpdate(product).Result;
            
            productEmailNotifications.SendEmailForProduct("ceva", addedProduct.Id);
            Assert.True(true);

            crudOnProducts.DeleteProduct(addedProduct.Id).Wait();
        }
    }
}