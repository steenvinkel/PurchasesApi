using System;
using Business.Models;
using Legacy.Repositories;

namespace Legacy.Services
{
    public class LegacyLossService(ILegacyLossRepository lossRepository,
        ILegacyPostingQueryRepository postingQueryRepository,
        ILegacyAccountStatusQueryRepository accountStatusQueryRepository) : ILegacyLossService
    {
        private readonly ILegacyLossRepository _lossRepository = lossRepository;
        private readonly ILegacyPostingQueryRepository _postingQueryRepository = postingQueryRepository;
        private readonly ILegacyAccountStatusQueryRepository _accountStatusQueryRepository = accountStatusQueryRepository;

        public void CalculateLoss(int userId, MonthAndYear monthAndYear)
        {
            var (startSum, endSum) = _accountStatusQueryRepository.StartAndEndOfMonthAccountStatusSum(userId, monthAndYear);
            var monthlyChange = _postingQueryRepository.GetMonthlyChange(userId, monthAndYear);

            var loss = Math.Round(monthlyChange + startSum - endSum, 2);

            _lossRepository.UpdateLoss(userId, monthAndYear, loss);
        }
    }
}
