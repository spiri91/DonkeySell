using System.Linq;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.ViewModels;
using DonkeySellApi.Workers;
using Ninject;
using NUnit.Framework;

namespace DonkeySell.Tests.Unit_tests
{
    [TestFixture]
    class CrudOnFavoritesTests
    {
        private ICrudOnFavorites crudOnFavorites;
        private ICrudOnProducts crudOnProducts;
        private ICrudOnUsers crudOnUsers;
        private string username;


        private int productId = 2;

        [SetUp]
        public void Initialize()
        {
            TestInitialiser.Initialise();
            crudOnFavorites = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnFavorites>();
            crudOnProducts = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnProducts>();
            crudOnUsers = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnUsers>();

            var viewUser = TestInitialiser.CreateUser();
            username = crudOnUsers.CreateOrUpdateUser(viewUser).Result.UserName;
        }

        [TearDown]
        public void TearDown()
        {
            crudOnUsers.DeleteUser(username).Wait();
        }

        [Test]
        public void ShouldAddFavoriteProduct()
        {
            var usf = crudOnFavorites.AddProductToFavorites(username, productId).Result;
            Assert.AreEqual(usf.Username, username);
            Assert.AreEqual(usf.ProductId, productId);
            crudOnFavorites.DeleteProductFromFavorites(username, productId);
        }

        [Test]
        public void ShouldDeleteFavoriteProduct()
        {
            var usf = crudOnFavorites.AddProductToFavorites(username, productId).Result;
            var deleted = crudOnFavorites.DeleteProductFromFavorites(username, productId).Result;
            Assert.IsTrue(deleted);
        }

        [Test]
        public void ShouldRetrieveFavoriteProducts()
        {
            var usf = crudOnFavorites.AddProductToFavorites(username, productId).Result;
            var pocos = crudOnFavorites.GetUsersFavoriteProducts(username).Result;
            Assert.IsNotNull(pocos);
            crudOnFavorites.DeleteProductFromFavorites(username, productId);
        }

        public void ShouldRevoteProductFromFavoritesAfterDeletingIt()
        {
            var productMockup = TestInitialiser.CreateProduct();
            var product = crudOnProducts.AddOrUpdate(productMockup).Result;
            crudOnFavorites.AddProductToFavorites(username, product.Id).Wait();
            crudOnProducts.DeleteProduct(productId).Wait();
            var favorites = crudOnFavorites.GetUsersFavoriteProducts(username).Result;
            Assert.AreEqual(favorites.Count(), 0);
        }
    }
}
