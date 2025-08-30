using Business.Models;
using Business.Repositories;
using Legacy.Mappers;
using Legacy.Models;
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
    public class LegacyAccountStatusController(IAccountStatusRepository accountStatusRepository, ILegacyLossService lossService) : ControllerBase
    {
        private readonly IAccountStatusRepository _accountStatusRepository = accountStatusRepository;
        private readonly ILegacyLossService _lossService = lossService;

        [HttpGet]
        public ActionResult<object> Get()
        {
            var accountStatuses = _accountStatusRepository.Get(HttpContext.GetUserId());

            return LegacyAccountStatusMapper.Map(accountStatuses);
        }

        [HttpPost]
        public ActionResult Post(List<LegacyAccountStatus> accountStatuses)
        {
            ValidateNotEmpty(accountStatuses);
            accountStatuses.ForEach(ValidatePost);

            var userId = HttpContext.GetUserId();

            var newAccountStatuses = _accountStatusRepository.Add(userId, accountStatuses.Select(a => a.Map()).ToList());

            var monthAndYear = new MonthAndYear(accountStatuses.First().Year, accountStatuses.First().Month);
            _lossService.CalculateLoss(userId, monthAndYear);

            return Ok(newAccountStatuses.Select(LegacyAccountStatusMapper.Map));
        }

        [HttpPut]
        public ActionResult Put(List<LegacyAccountStatus> accountStatuses)
        {
            ValidateNotEmpty(accountStatuses);
            accountStatuses.ForEach(ValidatePut);
            var userId = HttpContext.GetUserId();

            var existingAccountStatuses = _accountStatusRepository.Update(userId, accountStatuses.Select(a => a.Map()).ToList());

            var monthAndYear = new MonthAndYear(accountStatuses.First().Year, accountStatuses.First().Month);
            _lossService.CalculateLoss(userId, monthAndYear);

            return Ok(existingAccountStatuses.Select(LegacyAccountStatusMapper.Map));
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
                throw new ArgumentOutOfRangeException(nameof(accountStatus), $"The parameter {nameof(accountStatus.Account_status_id)} cannot be zero");
            }
            if (accountStatus.Account_id == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountStatus), $"The parameter {nameof(accountStatus.Account_id)} cannot be zero");
            }
        }

        private void ValidatePost(LegacyAccountStatus accountStatus)
        {
            if (accountStatus.Account_status_id != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountStatus), $"The parameter {nameof(accountStatus.Account_status_id)} should be zero");
            }
            if (accountStatus.Account_id == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountStatus), $"The parameter {nameof(accountStatus.Account_id)} cannot be zero");
            }
        }
    }
}
