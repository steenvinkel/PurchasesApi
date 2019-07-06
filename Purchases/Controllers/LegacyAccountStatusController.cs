using Business.Repositories;
using Legacy.LegacyFormatting;
using Legacy.Mappers;
using Legacy.Models;
using Legacy.Repositories;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;
using System;
using System.Collections.Generic;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyAccountStatusController : ControllerBase
    {
        private readonly IAccountStatusRepository _accountStatusRepository;
        private readonly ILegacyAccountStatusRepository _legacyAccountStatusRepository;

        public LegacyAccountStatusController(IAccountStatusRepository accountStatusRepository, ILegacyAccountStatusRepository legacyAccountStatusRepository)
        {
            _accountStatusRepository = accountStatusRepository;
            _legacyAccountStatusRepository = legacyAccountStatusRepository;
        }

        [HttpGet]
        public ActionResult<object> Get()
        {
            var accountStatuses = _accountStatusRepository.Get(HttpContext.GetUserId());

            var mapper = new AccountStatusMapper();

            return mapper.Map(accountStatuses).AddLegacyFormatting();
        }

        [HttpPost]
        public ActionResult Post(List<LegacyAccountStatus> accountStatuses)
        {
            accountStatuses.ForEach(ValidatePost);

            var userId = HttpContext.GetUserId();

            var newAccountStatuses = _legacyAccountStatusRepository.Post(accountStatuses, userId);

            return Ok(newAccountStatuses);
        }

        [HttpPut]
        public ActionResult Put(List<LegacyAccountStatus> accountStatuses)
        {
            accountStatuses.ForEach(ValidatePut);
            var userId = HttpContext.GetUserId();

            var existingAccountStatuses = _legacyAccountStatusRepository.Put(accountStatuses, userId);

            return Ok(existingAccountStatuses);
        }

        private void ValidatePut(LegacyAccountStatus accountStatus)
        {
            if (accountStatus.Account_status_id == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountStatus.Account_status_id));
            }
            if (accountStatus.Account_id == 0)
            {
                throw new ArgumentException(nameof(accountStatus.Account_id));
            }
        }

        private void ValidatePost(LegacyAccountStatus accountStatus)
        {
            if (accountStatus.Account_status_id != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountStatus.Account_status_id));
            }
            if (accountStatus.Account_id == 0)
            {
                throw new ArgumentException(nameof(accountStatus.Account_id));
            }
        }
    }
}
