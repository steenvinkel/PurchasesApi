using System;
using Business.Models;
using Business.Repositories;
using Legacy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/Account/{accountId}/[controller]")]
    [ApiController]
    public class AccountStatusController(IAccountStatusRepository accountStatusRepository, ILegacyLossService lossService) : ControllerBase
    {
        private readonly IAccountStatusRepository _accountStatusRepository = accountStatusRepository;
        private readonly ILegacyLossService _lossService = lossService;

        [HttpPost]
        public ActionResult Post(int accountId, AccountStatus accountStatus)
        {
            ValidatePost(accountStatus);

            var userId = HttpContext.GetUserId();

            _accountStatusRepository.Add(userId, accountId, accountStatus);

            var monthAndYear = new MonthAndYear(accountStatus.Date);
            _lossService.CalculateLoss(userId, monthAndYear);

            return Ok();
        }

        [HttpPut]
        public ActionResult Put(int accountId, AccountStatus accountStatus)
        {
            ValidatePut(accountStatus);

            var userId = HttpContext.GetUserId();

            _accountStatusRepository.Update(userId, accountId, accountStatus);

            var monthAndYear = new MonthAndYear(accountStatus.Date);
            _lossService.CalculateLoss(userId, monthAndYear);

            return Ok();
        }

        private void ValidatePost(AccountStatus accountStatus)
        {
            if (accountStatus.AccountStatusId != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountStatus), $"The parameter {nameof(accountStatus.AccountStatusId)} should be zero");
            }
        }

        private void ValidatePut(AccountStatus accountStatus)
        {
            if (accountStatus.AccountStatusId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(accountStatus), $"The parameter {nameof(accountStatus.AccountStatusId)} should not be zero");
            }
        }
    }
}