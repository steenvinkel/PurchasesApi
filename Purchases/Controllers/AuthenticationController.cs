using Business.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

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

        [HttpGet("ValidateToken")]
        public ActionResult ValidateToken(string authToken)
        {
            _authenticationService.GetUserIdAndExpiration(authToken);

            return Ok();
        }
    }
}