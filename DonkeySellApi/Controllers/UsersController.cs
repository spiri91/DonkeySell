using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using DonkeySellApi.Extra;
using DonkeySellApi.Models.ViewModels;
using DonkeySellApi.Workers;
using Microsoft.AspNet.Identity;

namespace DonkeySellApi.Controllers
{
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private ILogger logger;
        private ICrudOnUsers crudOnUsers;
        private IAuthorization authorization;
        private ICrudOnMessages crudOnMessages;

        public UsersController(ILogger logger, ICrudOnUsers crudOnUsers, IAuthorization authorization, ICrudOnMessages crudOnMessages)
        {
            this.crudOnUsers = crudOnUsers;
            this.logger = logger;
            this.crudOnMessages = crudOnMessages;
            this.authorization = authorization;
        }

        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody]ViewUser viewUser)
        {
            var newViewUSer = await crudOnUsers.CreateOrUpdateUser(viewUser);

            if (newViewUSer != null)
                return Ok(newViewUSer);

            return BadRequest();
        }

        [Authorize]
        [Route("{username}")]
        public async Task<IHttpActionResult> Delete(string username)
        {
            if (await authorization.UserIsHimself(User.Identity.GetUserName(), username))
            {
                var id = await crudOnUsers.DeleteUser(username);

                return Ok(id);
            }

            return Unauthorized();
        }

        [Route("{username}")]
        public async Task<IHttpActionResult> Get(string username)
        {
            var viewUSer = await crudOnUsers.GetUser(username);
            if (viewUSer != null)

                return Ok(viewUSer);

            return BadRequest();

        }

        [Authorize]
        [Route("{username}/Unread")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUnreadMessages(string username)
        {
            if (await authorization.UserIsHimself(User.Identity.GetUserName(), username))
            {
                var messages = await crudOnMessages.GetUnreadMessages(username);

                var returnedViewMessages = Mapper.Map<IEnumerable<ViewMessage>>(messages);

                return Ok(returnedViewMessages);
            }

            return Unauthorized();
        }

        [Authorize]
        [Route("{username}/MarkRead/{id:int}")]
        [HttpPost]
        public async Task<IHttpActionResult> MarkMessageAsRead(string username, int id)
        {
            if (await authorization.UserOwnsThisProduct(User.Identity.GetUserName(), username, id))
            {
                var message = crudOnMessages.MessageWasRead(id);
                var viewMessage = Mapper.Map<ViewMessage>(message);

                return Ok(viewMessage);
            }

            return Unauthorized();
        }

    }
}
