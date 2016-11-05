using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using DonkeySellApi.Extra;
using DonkeySellApi.Models.Shared;
using DonkeySellApi.Models.ViewModels;
using DonkeySellApi.Workers;
using Microsoft.AspNet.Identity;

namespace DonkeySellApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users/{username}/Favorites")]
    public class FavoriteController : ApiController
    {
        private ICrudOnFavorites crudOnFavorites;
        private IAuthorization authorization;

        public FavoriteController(ICrudOnFavorites crudOnFavorites, IAuthorization authorization)
        {
            this.crudOnFavorites = crudOnFavorites;
            this.authorization = authorization;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetFavorites(string username)
        {
                var products = await crudOnFavorites.GetUsersFavoriteProducts(username);
                var viewProducts = Mapper.Map<IEnumerable<ViewProduct>>(products);

                return Ok(viewProducts);
        }

        [Route("{productId:int}")]
        public async Task<IHttpActionResult> Post(string username, int productId)
        {
            if (await authorization.UserIsHimself(User.Identity.GetUserName(), username))
            {
                var usf = await crudOnFavorites.AddProductToFavorites(username, productId);
                var viewUsf = Mapper.Map<ViewUsersFavoriteProducts>(usf);

                return Ok(viewUsf);
            }

            return Unauthorized();
        }

        [Route("{productId:int}")]
        public async Task<IHttpActionResult> Delete(string username, int productid)
        {
            if (await authorization.UserIsHimself(User.Identity.GetUserName(), username))
            {
                var deleted = await crudOnFavorites.DeleteProductFromFavorites(username, productid);

                return Ok(deleted);
            }

            return Unauthorized();
        }
    }
}
