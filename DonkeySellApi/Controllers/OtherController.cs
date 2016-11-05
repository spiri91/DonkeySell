using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using DonkeySellApi.Extra;
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

        [HttpGet]
        [Route("categories")]
        public async Task<IEnumerable<ViewCategory>> GetCategories()
        {
            var categories = await getCitiesAndCategories.GetCategories();
            var viewCategories = Mapper.Map<IEnumerable<ViewCategory>>(categories);

            return viewCategories;
        }

        [HttpGet]
        [Route("cities")]
        public async Task<IEnumerable<ViewCity>> GetCities()
        {
            var cities = await getCitiesAndCategories.GetCities();
            var viewCities = Mapper.Map<IEnumerable<ViewCity>>(cities);

            return viewCities;
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
    }
}
