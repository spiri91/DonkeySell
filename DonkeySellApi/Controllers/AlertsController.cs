using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using AutoMapper;
using DonkeySellApi.Extra;
using DonkeySellApi.Models.Shared;
using DonkeySellApi.Models.ViewModels;
using DonkeySellApi.Workers;
using Microsoft.AspNet.Identity;

namespace DonkeySellApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users/{username}/Alerts")]
    public class AlertsController : ApiController
    {
        private ICrudOnAlerts crudOnAlerts;
        private IAuthorization authorization;
        private IThrowExceptionToUser throwExceptionToUser;
        private ICrudOnUsers crudOnUsers;

        public AlertsController(ICrudOnAlerts crudOnAlerts, IAuthorization authorization, IThrowExceptionToUser throwExceptionToUser, ICrudOnUsers crudOnUsers)
        {
            this.crudOnAlerts = crudOnAlerts;
            this.authorization = authorization;
            this.throwExceptionToUser = throwExceptionToUser;
            this.crudOnUsers = crudOnUsers;
        }

        [EnableQuery]
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAlerts(string username)
        {
            if (!await authorization.UserIsHimself(User.Identity.GetUserName(), username))
                return Unauthorized();

            try
            {
                var alerts = await crudOnAlerts.GetAlerts(username);
                var viewAlerts = Mapper.Map<IEnumerable<ViewAlert>>(alerts);

                return Ok(viewAlerts);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> PostAlert([FromBody] ViewAlert viewAlert)
        {
            try
            {
                var username = await crudOnUsers.GetUsernameById(viewAlert.UserId);

                if (!await authorization.UserIsHimself(User.Identity.GetUserName(), username))
                    return Unauthorized();

                var alert = Mapper.Map<Alert>(viewAlert);
                var newAlert = await crudOnAlerts.AddAlert(alert);
                var newViewAlert = Mapper.Map<ViewAlert>(newAlert);
                return Ok(newViewAlert);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> DeleteAlert(int id)
        {
            try
            {
                if (!await authorization.UserOwnsThisAlert(id, User.Identity.GetUserName()))
                    return Unauthorized();

                var deletedAlertId = await crudOnAlerts.DeleteAlert(id);

                return Ok(deletedAlertId);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }
    }
}
