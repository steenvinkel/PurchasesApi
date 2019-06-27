using Business.Repositories;
using Legacy.Mappers;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyAccountStatusController : ControllerBase
    {
        private readonly IAccountStatusRepository _accountStatusRepository;

        public LegacyAccountStatusController(IAccountStatusRepository accountStatusRepository)
        {
            _accountStatusRepository = accountStatusRepository;
        }

        [HttpGet]
        public ActionResult<object> Get()
        {
            var accountStatuses = _accountStatusRepository.Get(HttpContext.GetUserId());

            var mapper = new AccountStatusMapper();

            return mapper.Map(accountStatuses);
        }
    }
}
