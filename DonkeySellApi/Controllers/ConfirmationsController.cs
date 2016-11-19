using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using DonkeySellApi.Extra;
using DonkeySellApi.Workers;

namespace DonkeySellApi.Controllers
{
    [RoutePrefix("api/Confirmations")]
    public class ConfirmationsController : ApiController
    {
        private ICrudOnUsers crudOnUsers;
        private ILogger logger;

        public ConfirmationsController(ICrudOnUsers crudOnUsers, ILogger logger)
        {
            this.crudOnUsers = crudOnUsers;
            this.logger = logger;
        }

        [HttpGet]
        [Route("email/{username}/{guid}")]
        public async Task<HttpResponseMessage> ConfirmEmail(string username, string guid)
        {
            var response = new HttpResponseMessage();
            try
            {
                if (!await crudOnUsers.CheckIfGuidIsTheSame(guid, username))
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    return response;
                }

                await crudOnUsers.ConfirmEmail(username);

               
                response.Content = new StringContent("<html><body>Thanks "+ username +" you are now part of DonkeySell! </body></html>");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
            catch (Exception)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
    }
}
