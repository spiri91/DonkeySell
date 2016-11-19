using System;
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
        private IThrowExceptionToUser throwExceptionToUser;

        public MessagesController(ILogger logger, ICrudOnMessages crudOnMessages, IAuthorization authorization, IThrowExceptionToUser throwExceptionToUser)
        {
            this.logger = logger;
            this.crudOnMessages = crudOnMessages;
            this.authorization = authorization;
            this.throwExceptionToUser = throwExceptionToUser;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get(int productId)
        {
            try
            {
                var messages = await crudOnMessages.GetMessages(productId);
                var viewMessages = Mapper.Map<IEnumerable<ViewMessage>>(messages);

                return Ok(viewMessages);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Authorize]
        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody]ViewMessage viewMessage)
        {
            try
            {
                if (!await authorization.UserHasRightsOnMessage(User.Identity.GetUserName(), viewMessage.Id))
                    return Unauthorized();

                var message = Mapper.Map<Message>(viewMessage);
                var returnedMessage = await crudOnMessages.AddOrUpdate(message);
                var returnedViewMessage = Mapper.Map<ViewMessage>(returnedMessage);

                return Ok(returnedViewMessage);
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
                if (!await authorization.UserHasRightsOnMessage(User.Identity.GetUserName(), id))
                    return Unauthorized();

                int idOfMessage = await crudOnMessages.DeleteMessage(id);

                return Ok(idOfMessage);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }
    }
}
