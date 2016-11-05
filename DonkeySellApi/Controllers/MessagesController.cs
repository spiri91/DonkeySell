using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using DonkeySellApi.Extra;
using DonkeySellApi.Models;
using DonkeySellApi.Models.DatabaseModels;
using DonkeySellApi.Models.ViewModels;
using DonkeySellApi.Workers;
using Microsoft.AspNet.Identity;

namespace DonkeySellApi.Controllers
{
    [RoutePrefix("api/Products/{productId:int}/Messages")]
    public class MessagesController : ApiController
    {
        private ILogger logger;
        private ICrudOnMessages crudOnMessages;
        private IAuthorization authorization;
            
        public MessagesController(ILogger logger, ICrudOnMessages crudOnMessages, IAuthorization authorization)
        {
            this.logger = logger;
            this.crudOnMessages = crudOnMessages;
            this.authorization = authorization;
        }

        [Route("")]
        public async Task<IEnumerable<ViewMessage>> Get(int productId)
        {
            var messages = await crudOnMessages.GetMessages(productId);
            var viewMessages = Mapper.Map<IEnumerable<ViewMessage>>(messages);

            return viewMessages;

        }

        [Authorize]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody]ViewMessage viewMessage)
        {
            if (await  authorization.UserHasRightsOnMessage(User.Identity.GetUserName(), viewMessage.Id))
            {
                var message = Mapper.Map<Message>(viewMessage);
                var returnedMessage = await crudOnMessages.AddOrUpdate(message);
                var returnedViewMessage = Mapper.Map<ViewMessage>(returnedMessage);

                return Ok(returnedViewMessage);
            }

            return Unauthorized();
        }

        [Authorize]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (await authorization.UserHasRightsOnMessage(User.Identity.GetUserName(), id))
            {
                int idOfMessage = await crudOnMessages.DeleteMessage(id);

                return Ok(idOfMessage);
            }

            return Unauthorized();
        }
    }
}
