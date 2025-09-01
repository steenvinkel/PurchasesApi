using System.Collections.Generic;
using Business.Models;
using Business.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountStatusController(IAccountStatusRepository accountStatusRepository) : ControllerBase
    {
        private readonly IAccountStatusRepository _accountStatusRepository = accountStatusRepository;

        [HttpGet]
        public ActionResult<List<AccountStatusDAO>> Get()
        {
            return _accountStatusRepository.Get(HttpContext.GetUserId());
        }
    }
}