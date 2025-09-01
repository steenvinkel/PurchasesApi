using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        [HttpGet]
        public ActionResult<User> Get()
        {
            var userId = HttpContext.GetUserId();

            var user = userService.Get(userId);

            return Ok(user);
        }
    }
}
