using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DonkeySellApi.Extra
{
    public class DonkeySellDatabaseFunctions: IDisposable
    {
        private DonkeySellContext context;
        private UserManager<DonkeySellUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public DonkeySellDatabaseFunctions()
        {
            context = new DonkeySellContext();
            userManager = new UserManager<DonkeySellUser>(new UserStore<DonkeySellUser>(context));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        public bool Seed()
        {
            CreateCities();

            CreateCategories();

            for (var i = 0; i < 10; i++)
            {
                var random = Guid.NewGuid().ToString().Substring(0, 5);
                string username = "spiri" + random;
                string email = "testing" + random + "@test.com";
                string password = "Superme!" + random;

                var user = CreateDonkeySellUser(username, password, email);

                var product = CreateProduct(user);

                var message = CreateMessage(user, product);

                user.Products = new List<Product>() { product };
                message.Product = product;

                context.Messages.Add(message);
                context.Products.Add(product);

                Thread.Sleep(100);
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception("Failed to seed");
            }

            return true;
        }

        private void CreateCategories()
        {
            string categories = "Phones,Cars,Apartments,Animals,Electronics,Other";
            foreach (var category in categories.Split(','))
            {
                context.Categories.Add(new Category() {Name = category});
            }

            context.SaveChanges();
        }

        private void CreateCities()
        {
            string cities = "Alba,Arad,Arges,Bacau,Bihor,Bistrița-Nasaud,Botosani,Brasov,Braila,Bucuresti,Buzau,Calarasi,Cluj,Constanta,Covasna"+
                               "Dambovita,Dolj,Galati,Giurgiu,Gorj,Harghita,Hunedoara,Ialomita,Iasi,Ilfov,Maramures,Mehedinti,Mures,Neamt,Olt,Prahova"+
                               "Satu Mare,Salaj,Sibiu,Suceava,Teleorman,Timis,Tulcea,Vaslui,Valcea,Vrancea";
            foreach (var category in cities.Split(','))
            {
                context.Cities.Add(new City() { Name = category });
            }

            context.SaveChanges();
        }

        private Message CreateMessage(DonkeySellUser user, Product product)
        {
            Message message = new Message() { UserId = user.UserId, UserName = user.UserName, DateCreated = DateTime.Now, Value = "This is a comment", ProductId = product.Id };

            return message;
        }

        private DonkeySellUser CreateDonkeySellUser(string username, string email, string password)
        {

            var roles = new List<string> { "Active", "Admin", "AccountManager", "User" };
            foreach (string role in roles)
            {
                if (!roleManager.RoleExists(role))
                {
                    roleManager.Create(new IdentityRole(role));
                }
            }

            Guid guid = Guid.NewGuid();
            DonkeySellUser user = new DonkeySellUser() { UserName = username, Email = email, UserId = guid.ToString() };

            userManager.Create(user, password);

            return user;
        }

        private Product CreateProduct(DonkeySellUser user)
        {
            var categoryId = context.Categories.Single(x => x.Name == "Other").Id;
            var cityId = context.Cities.Single(x => x.Name == "Bacau").Id;

            Product product = new Product()
            {
                Title = "One plus 3",
                CityId = cityId,
                DatePublished = DateTime.Today,
                Description = "no description!",
                UserName = user.UserName,
                CategoryId = categoryId,
                Price = 1900,
                TradesAccepted = true
            };

            return product;
        }
        
        public void Dispose()
        {
            context.Dispose();
        }
    }
}
