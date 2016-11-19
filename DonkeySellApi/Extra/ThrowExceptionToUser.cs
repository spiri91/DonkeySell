using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace DonkeySellApi.Extra
{
    public interface IThrowExceptionToUser
    {
        IHttpActionResult Throw(Exception ex);

    }
    public class ThrowExceptionToUser : ApiController, IThrowExceptionToUser
    {
        public IHttpActionResult Throw(Exception ex)
        {
            if (ex is FormatException)
                return BadRequest("Invalid format exception");
            if (ex is ObjectNotFoundException)
                return NotFound();

            return InternalServerError();
        }
    }
}