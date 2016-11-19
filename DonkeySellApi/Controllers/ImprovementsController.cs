using System;
using System.Threading.Tasks;
using System.Web.Http;
using DonkeySellApi.Extra;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Workers;

namespace DonkeySellApi.Controllers
{
    [RoutePrefix("api/Improvements")]
    public class ImprovementsController : ApiController
    {
        private ICrudOnImprovements crudOnImprovements;
        private IThrowExceptionToUser throwExceptionToUser;

        public ImprovementsController(ICrudOnImprovements crudOnImprovements, IThrowExceptionToUser throwExceptionToUser)
        {
            this.crudOnImprovements = crudOnImprovements;
            this.throwExceptionToUser = throwExceptionToUser;
        }

        [Route("")]
        public async Task<IHttpActionResult> Post(Improvement improvement)
        {
            try
            {
                await crudOnImprovements.AddImprovement(improvement);
                return Ok();
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }
    }
}
