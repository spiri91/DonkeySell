using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using WebGrease.Css.Extensions;

namespace DonkeySellApi.Workers
{
    public interface ICrudOnProducts
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProduct(int id);
        Task<Product> AddOrUpdate(Product product);
        Task<int> DeleteProduct(int id);
        Task DeleteAllProductsOfUser(string username);
        Task<List<Product>> GetProductsOfUser(string username);
        Task<List<Product>> GetProductsByQuery(string query, int take, int skip, string orderedBy);
    }

    public class CrudOnProducts : ICrudOnProducts
    {
        private DonkeySellContext context;
        private ICrudOnMessages crudOnMessages;
        public CrudOnProducts(ICrudOnMessages crudOnMessages)
        {
            context = new DonkeySellContext();
            this.crudOnMessages = crudOnMessages;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return context.Products.ToList();
        }

        public async Task<Product> GetProduct(int id)
        {
            return context.Products.Single(x => x.Id == id);
        }

        public async Task<Product> AddOrUpdate(Product product)
        {
            if (product.Id != 0 || context.Products.Any(x => x.Id == product.Id))
            {
                var poco = context.Products.Single(x => x.Id == product.Id);
                await ClearProductOfImages(product.Id);

                poco.City = product.City;
                poco.Category = product.Category;
                poco.Title = product.Title;
                poco.Price = product.Price;
                poco.TradesAccepted = poco.TradesAccepted;
                poco.Rental = poco.Rental;
                poco.Free = poco.Free;
                poco.Description = product.Description;
                poco.Images = product.Images;
                await context.SaveChangesAsync();

                return poco;
            }

            product.DatePublished = DateTime.Now;
            product.UserId = context.Users.Single(x => x.UserName == product.UserName).UserId;
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return product;
        }

        private async Task ClearProductOfImages(int id)
        {
            var productImages = context.Products.Single(x => x.Id == id).Images.ToList();
            productImages.ForEach(i =>
            {
                context.Images.Remove(i);
            });
        }

        public async Task<int> DeleteProduct(int id)
        {
            var poco = context.Products.Single(x => x.Id == id);
            await crudOnMessages.DeleteAllMessagesForProduct(id);
            await DeleteProductFromFavorites(id);
            context.Products.Remove(poco);
            await context.SaveChangesAsync();

            return poco.Id;
        }

        public async Task DeleteAllProductsOfUser(string username)
        {
            var pocos = context.Products.Where(x => x.UserName == username).ToList();
            pocos.ForEach(async p =>
            {
                await crudOnMessages.DeleteAllMessagesForProduct(p.Id);
                await DeleteProductFromFavorites(p.Id);
                context.Products.Remove(p);
            });

            await  context.SaveChangesAsync();
        }

        private async Task DeleteProductFromFavorites(int id)
        {
            var favorites = context.UsersFavoriteProducts.Where(x => x.ProductId == id).ToList();
            favorites.ForEach(p =>
            {
                context.UsersFavoriteProducts.Remove(p);
            });
        }

        public async Task<List<Product>> GetProductsOfUser(string username)
        {
            var poco = context.Products.Where(x => x.UserName == username);

            return poco.ToList();
        }

        public async Task<List<Product>> GetProductsByQuery(string query, int take, int skip, string orderedBy)
        {
            var products = query != null ? context.Products.SqlQuery(query).ToList() : context.Products.ToList();

            return products.OrderByDescending(x => x.GetType().GetProperty(orderedBy).GetValue(x, null)).Skip(skip).Take(take).ToList();
        }
    }
}