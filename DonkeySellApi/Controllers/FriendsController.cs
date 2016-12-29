using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using DonkeySellApi.Extra;

namespace DonkeySellApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users/{username}/Friends")]
    public class FriendsController : ApiController
    {
        private ICrudOnFriends crudOnFriends;
        private IThrowExceptionToUser throwExceptionToUser;

        public FriendsController(ICrudOnFriends crudOnFriends, IThrowExceptionToUser throwExceptionToUser)
        {
            this.crudOnFriends = crudOnFriends;
            this.throwExceptionToUser = throwExceptionToUser;
        }

        [EnableQuery]
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetFriends(string username)
        {
            try
            {
                var friends = await crudOnFriends.GetFriendsOfUser(username);

                return Ok(friends);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [HttpPost]
        [Route("{friend}")]
        public async Task<IHttpActionResult> PostFriend(string username, string friend)
        {
            try
            {
                var newFriend = await crudOnFriends.AddUserFriend(username, friend);

                return Ok(newFriend);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }

        [HttpDelete]
        [Route("{friend}")]
        public async Task<IHttpActionResult> DeleteFriend(string username, string friend)
        {
            try
            {
                var id = await crudOnFriends.DeleteFriendFromUser(username, friend);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return throwExceptionToUser.Throw(ex);
            }
        }
    }
}
