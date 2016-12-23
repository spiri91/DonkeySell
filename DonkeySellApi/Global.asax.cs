using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.Shared;
using DonkeySellApi.Models.ViewModels;
using Newtonsoft.Json.Serialization;

namespace DonkeySellApi
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HttpConfiguration config = GlobalConfiguration.Configuration;
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            RegisterModelsForAutoMapper();
        }

        private static void RegisterModelsForAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<City, ViewCity>().ReverseMap();
                cfg.CreateMap<Category, ViewCategory>().ReverseMap();
                cfg.CreateMap<Product, ViewProduct>().ReverseMap();
                cfg.CreateMap<DonkeySellUser, ViewUser>().ForMember(x => x.Password, y => y.Ignore()).ReverseMap();
                cfg.CreateMap<Message, ViewMessage>().ReverseMap();
                cfg.CreateMap<UsersFavoriteProducts, ViewUsersFavoriteProducts>().ReverseMap();
                cfg.CreateMap<Alert, ViewAlert>().ReverseMap();
            });
            Mapper.AssertConfigurationIsValid();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var response = context.Response;

            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("X-Frame-Options", "ALLOW-FROM *");

            if (context.Request.HttpMethod == "OPTIONS")
            {
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST, DELETE, PUT, TOKEN");
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization");
                response.AddHeader("Access-Control-Max-Age", "1728000");
                response.End();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}