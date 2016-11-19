using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using AutoMapper;
using DonkeySellApi.Extra;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.ViewModels;
using DonkeySellApi.Workers;
using Microsoft.AspNet.Identity;

namespace DonkeySellApi.Controllers
{
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        private ILogger logger;
        private ICrudOnProducts crudOnProducts;
        private IAuthorization authorization;
        private IMyQueryBuilder myQueryBuilder;
        private ICrudOnFavorites crudOnFavorites;
        private IThrowExceptionToUser throwExceptionToUser;

        public ProductsController(ILogger logger, ICrudOnProducts crudOnProducts, IAuthorization authorization, IMyQueryBuilder myQueryBuilder, ICrudOnFavorites crudOnFavorites, IThrowExceptionToUser throwExceptionToUser)
        {
            this.logger = logger;
            this.crudOnProducts = crudOnProducts;
            this.authorization = authorization;
            this.myQueryBuilder = myQueryBuilder;
            this.crudOnFavorites = crudOnFavorites;
            this.throwExceptionToUser = throwExceptionToUser;
        }

        [Route("")]
        [EnableQuery]
        public async Task<IEnumerable<ViewProduct>> Get()
        {
            var products = await crudOnProducts.GetAllProducts();
            var viewProducts = Mapper.Map<IEnumerable<ViewProduct>>(products);

            return viewProducts;
        }

        [Route("query/{givenQuery}/{take:int}/{skip:int}/{orderedBy}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetByQuery(string givenQuery, int take, int skip, string orderedBy)
        {
            string query = null;
            try
            {
                if (givenQuery != "all")
                    query = await myQueryBuilder.BuildQuery(givenQuery);

                var products = await crudOnProducts.GetProductsByQuery(query, take, skip, orderedBy);
                var viewProducts = Mapper.Map<IEnumerable<ViewProduct>>(products);

                return Ok(viewProducts);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Route("{id:int}")]
        [EnableQuery]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                var product = await crudOnProducts.GetProduct(id);
                var viewProduct = Mapper.Map<ViewProduct>(product);

                return Ok(viewProduct);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Route("{username}")]
        public async Task<IHttpActionResult> Get(string username)
        {
            try
            {
                var products = await crudOnProducts.GetProductsOfUser(username);
                var viewProducts = Mapper.Map<IEnumerable<ViewProduct>>(products);

                return Ok(viewProducts);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Authorize]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody]ViewProduct viewProduct)
        {
            try
            {
                if (!await authorization.UserCanUpdateProduct(User.Identity.GetUserName(), viewProduct.Id, viewProduct.UserName))
                    return Unauthorized();

                var product = Mapper.Map<Product>(viewProduct);
                var returnedProduct = await crudOnProducts.AddOrUpdate(product);
                var returnedViewProduct = Mapper.Map<ViewProduct>(returnedProduct);

                return Ok(returnedViewProduct);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Authorize]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                if (!await authorization.UserCanDeleteProduct(User.Identity.GetUserName(), id))
                    return Unauthorized();

                await crudOnFavorites.DeleteProductFromAllUsers(id);
                int idOfProduct = await crudOnProducts.DeleteProduct(id);

                return Ok(idOfProduct);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }
    }
}
