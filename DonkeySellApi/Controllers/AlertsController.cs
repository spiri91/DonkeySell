using System;
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
    [RoutePrefix("api/Users/{username}/Alerts")]
    public class AlertsController : ApiController
    {
        private ICrudOnAlerts crudOnAlerts;
        private IAuthorization authorization;
        private IThrowExceptionToUser throwExceptionToUser;

        public AlertsController(ICrudOnAlerts crudOnAlerts, IAuthorization authorization, IThrowExceptionToUser throwExceptionToUser)
        {
            this.crudOnAlerts = crudOnAlerts;
            this.authorization = authorization;
            this.throwExceptionToUser = throwExceptionToUser;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAlerts(string username)
        {
            if (!await authorization.UserIsHimself(User.Identity.GetUserName(), username))
                return Unauthorized();

            try
            {
                var alerts = await crudOnAlerts.GetAlerts(username);

                return Ok(alerts);
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
                if (! await authorization.UserIsHimself(User.Identity.GetUserId(), viewAlert.UserId))
                    return Unauthorized();

                var alert = Mapper.Map<Alert>(viewAlert);
                var newAlert = await crudOnAlerts.AddAlert(alert);
                return Ok(newAlert);
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
                if (! await authorization.UserOwnsThisAlert(id, User.Identity.GetUserId()))
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
