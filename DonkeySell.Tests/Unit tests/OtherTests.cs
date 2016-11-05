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

        [SetUp]
        public void Initialize()
        {
            TestInitialiser.Initialise();
            this.getCitiesAndCategories = TestInitialiser.ninjectKernel.kernel.Get<IGetCitiesAndCategories>();
           
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
    }
}