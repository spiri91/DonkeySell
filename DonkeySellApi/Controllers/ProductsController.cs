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

        public ProductsController(ILogger logger, ICrudOnProducts crudOnProducts, IAuthorization authorization, IMyQueryBuilder myQueryBuilder, ICrudOnFavorites crudOnFavorites)
        {
            this.logger = logger;
            this.crudOnProducts = crudOnProducts;
            this.authorization = authorization;
            this.myQueryBuilder = myQueryBuilder;
            this.crudOnFavorites = crudOnFavorites;
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
        public async Task<IEnumerable<ViewProduct>> GetByQuery(string givenQuery, int take, int skip, string orderedBy)
        {
            string query = null;
            if (givenQuery != "all")
                query = await myQueryBuilder.BuildQuery(givenQuery);
            
            var products = await crudOnProducts.GetProductsByQuery(query, take, skip, orderedBy);
            var viewProducts = Mapper.Map<IEnumerable<ViewProduct>>(products);

            return viewProducts;
        }

        [Route("{id:int}")]
        [EnableQuery]
        public async Task<IHttpActionResult> Get(int id)
        {
            var product = await crudOnProducts.GetProduct(id);
            if (product != null)
            {
                var viewProduct = Mapper.Map<ViewProduct>(product);

                return Ok(viewProduct);
            }

            return BadRequest();
        }

        [Route("{username}")]
        public async Task<IEnumerable<ViewProduct>> Get(string username)
        {
            var products = await crudOnProducts.GetProductsOfUser(username);
            var viewProducts = Mapper.Map<IEnumerable<ViewProduct>>(products);

            return viewProducts;
        }

        [Authorize]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody]ViewProduct viewProduct)
        {
            if (await authorization.UserCanUpdateProduct(User.Identity.GetUserName(), viewProduct.Id, viewProduct.UserName))
            {
                var product = Mapper.Map<Product>(viewProduct);
                var returnedProduct = await crudOnProducts.AddOrUpdate(product);
                var returnedViewProduct = Mapper.Map<ViewProduct>(returnedProduct);

                return Ok(returnedViewProduct);
            }

            return Unauthorized();
        }

        [Authorize]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (await authorization.UserCanDeleteProduct(User.Identity.GetUserName(), id))
            {
                await crudOnFavorites.DeleteProductFromAllUsers(id);
                int idOfProduct = await crudOnProducts.DeleteProduct(id);

                return Ok(idOfProduct);
            }

            return Unauthorized();
        }
    }
}
