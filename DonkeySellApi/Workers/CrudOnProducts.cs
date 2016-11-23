using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.Wrapers;

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
        Task<ProductsAndCount> GetProductsByQuery(string query, int take, int skip, string orderedBy, SortDirection sortDirection = SortDirection.Descending);
    }

    public class CrudOnProducts : ICrudOnProducts, IDisposable
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
            if (!context.Products.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            return context.Products.Single(x => x.Id == id);
        }

        public async Task<Product> AddOrUpdate(Product product)
        {
            if (!product.IsValid())
                throw new FormatException();

            if (product.Id == 0)
                return await CreateProduct(product);

            if (!context.Products.Any(x => x.Id == product.Id))
                throw new ObjectNotFoundException();

            return await UpdateProduct(product);
        }

        private async Task<Product> CreateProduct(Product product)
        {
            if(!context.Users.Any(x => x.UserName == product.UserName))
                throw new ObjectNotFoundException();

            product.DatePublished = DateTime.Now;
            product.UserId = context.Users.Single(x => x.UserName == product.UserName).UserId;
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return product;
        }

        private async Task<Product> UpdateProduct(Product product)
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
            if (!context.Products.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            var poco = context.Products.Single(x => x.Id == id);
            await crudOnMessages.DeleteAllMessagesForProduct(id);
            await DeleteProductFromFavorites(id);
            await DeleteImagesOfProduct(poco.Images.ToList());
            context.Products.Remove(poco);
            await context.SaveChangesAsync();

            return poco.Id;
        }

        private async Task DeleteImagesOfProduct(List<Image> images)
        {
            images.ForEach(x =>
            {
                context.Images.Remove(x);
            });
        }

        public async Task DeleteAllProductsOfUser(string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var pocos = context.Products.Where(x => x.UserName == username).ToList();
            pocos.ForEach(async p =>
            {
                await crudOnMessages.DeleteAllMessagesForProduct(p.Id);
                await DeleteProductFromFavorites(p.Id);
                context.Products.Remove(p);
            });

            await context.SaveChangesAsync();
        }

        private async Task DeleteProductFromFavorites(int id)
        {
            if (!context.Products.Any(x => x.Id == id))
                throw new ObjectNotFoundException();

            var favorites = context.UsersFavoriteProducts.Where(x => x.ProductId == id).ToList();
            favorites.ForEach(p =>
            {
                context.UsersFavoriteProducts.Remove(p);
            });
        }

        public async Task<List<Product>> GetProductsOfUser(string username)
        {
            if (!context.Users.Any(x => x.UserName == username))
                throw new ObjectNotFoundException();

            var poco = context.Products.Where(x => x.UserName == username);

            return poco.ToList();
        }

        public async Task<ProductsAndCount> GetProductsByQuery(string query, int take, int skip, string orderedBy, SortDirection sortDirection = SortDirection.Descending)
        {
            var dbProducts = query != null ? context.Products.SqlQuery(query).ToList() : context.Products.ToList();
            var count = dbProducts.Count();

            var simpleProducts = dbProducts.Select(CreateSimpleProduct);

            var products = sortDirection == SortDirection.Descending
                ? simpleProducts.OrderByDescending(x => x.GetType().GetProperty(orderedBy).GetValue(x, null))
                : simpleProducts.OrderBy(x => x.GetType().GetProperty(orderedBy).GetValue(x, null));

            return new ProductsAndCount() {Count = count, Products = products.Skip(skip).Take(take).ToList()};
        }

        private static Product CreateSimpleProduct(Product x)
        {
            return new Product()
            {
                Id = x.Id,
                Title = x.Title,
                Price = x.Price,
                UserName = x.UserName,
                Category = x.Category,
                DatePublished = x.DatePublished,
                City = x.City,
                Images = x.Images.Any() ? new Collection<Image>() {x.Images.ToList()[0]} : null
            };
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}