using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.Shared;
using DonkeySellApi.Models.ViewModels;
using WebGrease.Css.Extensions;

namespace DonkeySellApi.Workers
{
    public interface ICrudOnFavorites
    {
        Task<UsersFavoriteProducts> AddProductToFavorites(string username, int productId);

        Task<bool> DeleteProductFromFavorites(string username, int productId);

        Task<bool> DeleteProductFromAllUsers(int productId);

        Task<List<Product>> GetUsersFavoriteProducts(string username);
    }

    public class CrudOnFavorites: ICrudOnFavorites
    {
        // ufp = usersFavoriteProduct

        private DonkeySellContext context;
        private ICrudOnProducts crudOnProducts;

        public CrudOnFavorites(ICrudOnProducts crudOnProducts)
        {
            context = new DonkeySellContext();
            this.crudOnProducts = crudOnProducts;
        }

        public async Task<UsersFavoriteProducts> AddProductToFavorites(string username, int productId)
        {
            if (! await ProductIsAllreadyAdded(username, productId))
            {
                var usf = new UsersFavoriteProducts() {ProductId = productId, Username = username};
                context.UsersFavoriteProducts.Add(usf);
                await context.SaveChangesAsync();
                return usf;
            }

            return null;
        }

        private async Task<bool> ProductIsAllreadyAdded(string username, int productId)
        {
            return context.UsersFavoriteProducts.Any(x => x.ProductId == productId && x.Username == username);
        }

        public async Task<bool> DeleteProductFromFavorites(string username, int productId)
        {
            var usf = context.UsersFavoriteProducts.Single(x => x.ProductId == productId && x.Username == username);
            context.UsersFavoriteProducts.Remove(usf);
            bool deleted = await context.SaveChangesAsync() > 0;

            return deleted;
        }

        public async Task<bool> DeleteProductFromAllUsers(int productId)
        {
            var usfs = context.UsersFavoriteProducts.Where(x => x.ProductId == productId);
            usfs.ForEach(x =>
            {
               context.UsersFavoriteProducts.Remove(x);
            });

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Product>> GetUsersFavoriteProducts(string username)
        {
            var usfs = context.UsersFavoriteProducts.Where(x => x.Username == username).ToList();
            var products = new List<Product>();
            usfs.ForEach(async x =>
            {
                var product = await crudOnProducts.GetProduct(x.ProductId);
                products.Add(product);
            });

            return products;
        }
    }
}