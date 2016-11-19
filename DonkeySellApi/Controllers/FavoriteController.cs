using System;
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
        private IThrowExceptionToUser throwExceptionToUser;

        public FavoriteController(ICrudOnFavorites crudOnFavorites, IAuthorization authorization, IThrowExceptionToUser throwExceptionToUser)
        {
            this.crudOnFavorites = crudOnFavorites;
            this.authorization = authorization;
            this.throwExceptionToUser = throwExceptionToUser;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetFavorites(string username)
        {
            try
            {
                var products = await crudOnFavorites.GetUsersFavoriteProducts(username);
                var viewProducts = Mapper.Map<IEnumerable<ViewProduct>>(products);

                return Ok(viewProducts);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Route("{productId:int}")]
        public async Task<IHttpActionResult> Post(string username, int productId)
        {
            if (!await authorization.UserIsHimself(User.Identity.GetUserName(), username))
                return Unauthorized();

            try
            {
                var usf = await crudOnFavorites.AddProductToFavorites(username, productId);
                var viewUsf = Mapper.Map<ViewUsersFavoriteProducts>(usf);

                return Ok(viewUsf);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }

        }

        [Route("{productId:int}")]
        public async Task<IHttpActionResult> Delete(string username, int productid)
        {
            try
            {
                if (!await authorization.UserIsHimself(User.Identity.GetUserName(), username))
                    return Unauthorized();

                var deleted = await crudOnFavorites.DeleteProductFromFavorites(username, productid);

                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }
    }
}
