using System;
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
        private IMyPasswordGenerator myPasswordGenerator;
        private IMailSender mailSender;
        private IThrowExceptionToUser throwExceptionToUser;

        public UsersController(ILogger logger, ICrudOnUsers crudOnUsers, IAuthorization authorization, ICrudOnMessages crudOnMessages, IMyPasswordGenerator myPasswordGenerator, IMailSender mailSender, IThrowExceptionToUser throwExceptionToUser)
        {
            this.crudOnUsers = crudOnUsers;
            this.logger = logger;
            this.crudOnMessages = crudOnMessages;
            this.authorization = authorization;
            this.mailSender = mailSender;
            this.myPasswordGenerator = myPasswordGenerator;
            this.throwExceptionToUser = throwExceptionToUser;
        }

        [Route("")]
        public async Task<IHttpActionResult> Post([FromBody]ViewUser viewUser)
        {
            try
            {
                var newUSer = await crudOnUsers.CreateOrUpdateUser(viewUser);
                var newViewUser = Mapper.Map<ViewUser>(newUSer);
                return Ok(newViewUser);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Authorize]
        [Route("{username}")]
        public async Task<IHttpActionResult> Delete(string username)
        {
            if (!await authorization.UserIsHimself(User.Identity.GetUserName(), username))
                return Unauthorized();

            try
            {
                var id = await crudOnUsers.DeleteUser(username);

                return Ok(id);
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
                var user = await crudOnUsers.GetUser(username);
                var viewUser = Mapper.Map<ViewUser>(user);

                return Ok(viewUser);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Authorize]
        [Route("{username}/Unread")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUnreadMessages(string username)
        {
            if (!await authorization.UserIsHimself(User.Identity.GetUserName(), username))
                return Unauthorized();

            try
            {
                var messages = await crudOnMessages.GetUnreadMessages(username);
                var returnedViewMessages = Mapper.Map<IEnumerable<ViewMessage>>(messages);

                return Ok(returnedViewMessages);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [Authorize]
        [Route("{username}/MarkRead/{id:int}")]
        [HttpPost]
        public async Task<IHttpActionResult> MarkMessageAsRead(string username, int id)
        {
            try
            {
                if (!await authorization.UserOwnsThisProduct(User.Identity.GetUserName(), username, id))
                    return Unauthorized();

                var message = await crudOnMessages.MessageWasRead(id);
                var viewMessage = Mapper.Map<ViewMessage>(message);

                return Ok(viewMessage);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [HttpPost]
        [Route("{username}/changePassword")]
        public async Task<IHttpActionResult> ChangePassword(string username, [FromBody]ResetPassword resetPassword)
        {
            try
            {
                await crudOnUsers.ChangePassword(username, resetPassword.OldPassword, resetPassword.NewPassword);

                return Ok();
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [HttpPost]
        [Route("{username}/resetPassword")]
        public async Task<IHttpActionResult> ResetPassword(string username)
        {
            var newPassword = myPasswordGenerator.GeneratePassword();
            try
            {
                var email = await crudOnUsers.GetEmailOfUser(username);
                await crudOnUsers.ResetPassword(username, newPassword);
                await mailSender.SendNewPasswordMail(newPassword, email);

                return Ok();
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }
    }
}
