using System;
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
            });

            Mapper.AssertConfigurationIsValid();
        }

        public static ViewUser CreateUser()
        {
            var user = new ViewUser()
            {
                Address = "Bujoreni nr 14",
                Email = "DonkeyUser@donkey.com",
                UserName = "LittleDonkey",
                Password = "Super91!"
            };

            return user;
        }

        public static Product CreateProduct()
        {
            var viewProduct = new ViewProduct()
            {
                CityId = 2,
                Description = "nothing interesting here",
                UserName = "LittleDonkey",
                CategoryId = 4,
                Title = "One plus 2",
                Price = 1500,
                TradesAccepted = true
            };

            var product = Mapper.Map<Product>(viewProduct);

            return product;
        }

        public static Message CreateMessage()
        {
            ViewMessage viewMessage = new ViewMessage()
            {
                DateCreated = DateTime.Now,
                ProductId = 2,
                Value = "another comment here",
                UserName = "LittleDonkey"
            };

            var message = Mapper.Map<Message>(viewMessage);

            return message;
        }
    }
}
