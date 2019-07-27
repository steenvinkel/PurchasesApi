using System;
using Business.Models;
using Legacy.Repositories;

namespace Legacy.Services
{
    public class LegacyLossService : ILegacyLossService
    {
        private readonly ILegacyLossRepository _lossRepository;
        private readonly ILegacyPostingQueryRepository _postingQueryRepository;
        private readonly ILegacyAccountStatusQueryRepository _accountStatusQueryRepository;

        public LegacyLossService(ILegacyLossRepository lossRepository,
            ILegacyPostingQueryRepository postingQueryRepository,
            ILegacyAccountStatusQueryRepository accountStatusQueryRepository)
        {
            _lossRepository = lossRepository;
            _postingQueryRepository = postingQueryRepository;
            _accountStatusQueryRepository = accountStatusQueryRepository;
        }

        public void CalculateLoss(int userId, MonthAndYear monthAndYear)
        {
            var (startSum, endSum) = _accountStatusQueryRepository.StartAndEndOfMonthAccountStatusSum(userId, monthAndYear);
            var monthlyChange = _postingQueryRepository.GetMonthlyChange(userId, monthAndYear);

            var loss = Math.Round(monthlyChange + startSum - endSum, 2);

            _lossRepository.UpdateLoss(userId, monthAndYear, loss);
        }
    }
}
