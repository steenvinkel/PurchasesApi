using System.Collections.Generic;
using Business.Models;
using Business.Services;
using Legacy.LegacyFormatting;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyAccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public LegacyAccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public ActionResult<List<Account>> Get() => _accountService.Get(HttpContext.GetUserId()).AddLegacyFormatting();
    }
}