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

        public class Credentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        public ActionResult Login([FromBody] Credentials credentials)
        {
            var username = credentials.Username;
            var password = credentials.Password;
            bool authenticated = _authenticationService.IsValidCredentials(username, password);

            return authenticated
                ? Ok() as ActionResult
                : Unauthorized();
        }

        [HttpGet("ValidateToken")]
        public ActionResult ValidateToken(string authToken)
        {
            _authenticationService.GetUserIdAndExpiration(authToken);

            return Ok();
        }

        [HttpPost("ValidateToken")]
        public ActionResult PostValidateToken([FromBody] string authToken)
        {
            _authenticationService.GetUserIdAndExpiration(authToken);

            return Ok();
        }
    }
}