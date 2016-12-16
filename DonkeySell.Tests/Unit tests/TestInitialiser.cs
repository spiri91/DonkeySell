using System;
using System.Linq;
using AutoMapper;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.Shared;
using DonkeySellApi.Models.ViewModels;

namespace DonkeySell.Tests.Unit_tests
{
    public static class TestInitialiser
    {
        public static NinjectKernel ninjectKernel = new NinjectKernel();
        public static DonkeySellContext context = new DonkeySellContext();

        public static void Initialise()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Product, ViewProduct>().ReverseMap();
                cfg.CreateMap<DonkeySellUser, ViewUser>().ForMember(x => x.Password, y => y.Ignore()).ReverseMap();
                cfg.CreateMap<Message, ViewMessage>().ReverseMap();
                cfg.CreateMap<Category, ViewCategory>().ReverseMap();
                cfg.CreateMap<City, ViewCity>().ReverseMap();
                cfg.CreateMap<UsersFavoriteProducts, ViewUsersFavoriteProducts>().ReverseMap();
                cfg.CreateMap<Alert, ViewAlert>().ReverseMap();
            });

            Mapper.AssertConfigurationIsValid();
        }

        public static ViewUser CreateUser()
        {
            var user = new ViewUser()
            {
                Address = "Bujoreni nr 14",
                Email = "spataru.ionut91@yahoo.com",
                UserName = "LittleDonkey",
                Password = "Super9108"
            };

            return user;
        }

        public static Product CreateProduct()
        {
            var cityId = context.Cities.Where(x => x.Name == "Bacau").ToList()[0].Id;
            var categoryId = context.Categories.Where(x => x.Name == "Other").ToList()[0].Id;
            var viewProduct = new ViewProduct()
            {
                CityId = cityId,
                Description = "nothing interesting here just a description for this",
                UserName = "spiri1",
                CategoryId = categoryId,
                Title = "One plus 2",
                Price = 1500,
                TradesAccepted = true
            };

            var product = Mapper.Map<Product>(viewProduct);

            return product;
        }

        public static Message CreateMessage()
        {
            var productId = context.Products.ToList()[0].Id;
            ViewMessage viewMessage = new ViewMessage()
            {
                DateCreated = DateTime.Now,
                ProductId = productId,
                Value = "another comment here",
                UserName = "spiri1"
            };

            var message = Mapper.Map<Message>(viewMessage);

            return message;
        }
    }
}
