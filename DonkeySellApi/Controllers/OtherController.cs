using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using AutoMapper;
using DonkeySellApi.Extra;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.ViewModels;
using DonkeySellApi.Workers;

namespace DonkeySellApi.Controllers
{
    [RoutePrefix("api/Other")]
    public class OtherController : ApiController
    {
        private ILogger logger;
        private IGetCitiesAndCategories getCitiesAndCategories;
        private ICrudOnUsers crudOnUsers;

        public OtherController(ILogger logger, IGetCitiesAndCategories getCitiesAndCategories, ICrudOnUsers crudOnUsers)
        {
            this.logger = logger;
            this.getCitiesAndCategories = getCitiesAndCategories;
            this.crudOnUsers = crudOnUsers;
        }

        [EnableQuery]
        [HttpGet]
        [Route("categories")]
        public async Task<IEnumerable<Category>> GetCategories()
        {
            var categories = await getCitiesAndCategories.GetCategories();

            return categories;
        }

        [EnableQuery]
        [HttpGet]
        [Route("cities")]
        public async Task<IEnumerable<City>> GetCities()
        {
            var cities = await getCitiesAndCategories.GetCities();

            return cities;
        }

        [HttpGet]
        [Route("usernameIsNotTaken/{username}")]
        public async Task<bool> UsernameIsNotTaken(string username)
        {
            var usernameIsNotTaken = await crudOnUsers.CheckIfUsernameIsTaken(username);

            return usernameIsNotTaken;
        }

        [HttpGet]
        [Route("emailNotInUse/{email}")]
        public async Task<bool> EmailNotInUse(string email)
        {
            email = email.Replace('$', '.');
            var emailAddressNotInUse = await crudOnUsers.CheckIfEmailIsInUse(email);

            return emailAddressNotInUse;
        }

        [HttpGet]
        [Route("findUser/{username}")]
        public async Task<IEnumerable<string>> GetUsersLike(string username)
        {
            var users = await crudOnUsers.GetUsersLike(username);

            return users;
        }
    }
}
