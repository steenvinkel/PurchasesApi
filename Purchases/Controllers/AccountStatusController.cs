using System.Collections.Generic;
using System.Linq;
using Business.Models;
using Business.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountStatusController : ControllerBase
    {
        private readonly IAccountStatusRepository _accountStatusRepository;

        public AccountStatusController(IAccountStatusRepository accountStatusRepository)
        {
            _accountStatusRepository = accountStatusRepository;
        }

        [HttpGet]
        public ActionResult<List<AccountStatus>> Get()
        {
            return _accountStatusRepository.Get(HttpContext.GetUserId());
        }
    }
}