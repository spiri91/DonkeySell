using System.Linq;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Workers;
using Ninject;
using NUnit.Framework;

namespace DonkeySell.Tests.Unit_tests
{
    [TestFixture]
    public class CrudOnProductsTests
    {
        private ICrudOnProducts crudOnProducts;
        private Product product;

        [SetUp]
        public void Initialize()
        {
            TestInitialiser.Initialise();
            crudOnProducts = TestInitialiser.ninjectKernel.kernel.Get<ICrudOnProducts>();
            product = TestInitialiser.CreateProduct();
        }

        [Test]
        public void ShouldGetAllProducts()
        {
            var products = crudOnProducts.GetAllProducts();
            Assert.NotNull(products);
        }

        [Test]
        public void ShouldAddProduct()
        {
            var dbProduct = crudOnProducts.AddOrUpdate(product).Result;
            Assert.NotNull(dbProduct);
            crudOnProducts.DeleteProduct(dbProduct.Id).Wait();
        }

        [Test]
        public void ShouldUpdateProduct()
        {
            int newCityId = TestInitialiser.context.Cities.Where(x => x.Name == "Iasi").ToList()[0].Id;
            var dbProduct = crudOnProducts.AddOrUpdate(product).Result;
            dbProduct.CityId = newCityId;
            var updatedProduct = crudOnProducts.AddOrUpdate(dbProduct).Result;
            Assert.AreEqual(newCityId, updatedProduct.CityId);
            crudOnProducts.DeleteProduct(dbProduct.Id).Wait();
        }

        [Test]
        public void ShouldDeleteProduct()
        {
            var dbProduct = crudOnProducts.AddOrUpdate(product).Result;
            var deletedId = crudOnProducts.DeleteProduct(dbProduct.Id).Result;
            Assert.AreEqual(dbProduct.Id, deletedId);
        }
    }
}
