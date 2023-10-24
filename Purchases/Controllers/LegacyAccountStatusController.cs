using Business.Models;
using Business.Repositories;
using Legacy.Mappers;
using Legacy.Models;
using Legacy.Repositories;
using Legacy.Services;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyAccountStatusController : ControllerBase
    {
        private readonly IAccountStatusRepository _accountStatusRepository;
        private readonly ILegacyAccountStatusRepository _legacyAccountStatusRepository;
        private readonly ILegacyLossService _lossService;

        public LegacyAccountStatusController(IAccountStatusRepository accountStatusRepository, ILegacyAccountStatusRepository legacyAccountStatusRepository, ILegacyLossService lossService)
        {
            _accountStatusRepository = accountStatusRepository;
            _legacyAccountStatusRepository = legacyAccountStatusRepository;
            _lossService = lossService;
        }

        [HttpGet]
        public ActionResult<object> Get()
        {
            var accountStatuses = _accountStatusRepository.Get(HttpContext.GetUserId());

            var mapper = new AccountStatusMapper();

            return mapper.Map(accountStatuses);
        }

        [HttpPost]
        public ActionResult Post(List<LegacyAccountStatus> accountStatuses)
        {
            ValidateNotEmpty(accountStatuses);
            accountStatuses.ForEach(ValidatePost);

            var userId = HttpContext.GetUserId();

            var newAccountStatuses = _legacyAccountStatusRepository.Post(accountStatuses, userId);

            var monthAndYear = new MonthAndYear(accountStatuses.First().Year, accountStatuses.First().Month);
            _lossService.CalculateLoss(userId, monthAndYear);

            return Ok(newAccountStatuses);
        }

        [HttpPut]
        public ActionResult Put(List<LegacyAccountStatus> accountStatuses)
        {
            ValidateNotEmpty(accountStatuses);
            accountStatuses.ForEach(ValidatePut);
            var userId = HttpContext.GetUserId();

            var existingAccountStatuses = _legacyAccountStatusRepository.Put(accountStatuses, userId);

            var monthAndYear = new MonthAndYear(accountStatuses.First().Year, accountStatuses.First().Month);
            _lossService.CalculateLoss(userId, monthAndYear);

            return Ok(existingAccountStatuses);
        }

        private static void ValidateNotEmpty(List<LegacyAccountStatus> accountStatuses)
        {
            if (accountStatuses.Count == 0)
            {
                throw new ArgumentException(nameof(accountStatuses) + " was empty");
            }
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
