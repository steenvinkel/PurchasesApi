using Microsoft.AspNetCore.Mvc;
using System;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        [HttpGet]
        public ActionResult SaveAuthToken(string authToken)
        {
            if (authToken == null)
            {
                throw new ArgumentNullException(nameof(authToken));
            }

            Response.Cookies.Append("auth_token", authToken);

            return Ok();
        }
    }
}