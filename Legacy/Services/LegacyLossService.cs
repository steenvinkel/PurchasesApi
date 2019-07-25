using System;
using Business.Models;
using Legacy.Repositories;

namespace Legacy.Services
{
    public class LegacyLossService : ILegacyLossService
    {
        private readonly ILegacyLossRepository _lossRepository;

        public LegacyLossService(ILegacyLossRepository lossRepository)
        {
            _lossRepository = lossRepository;
        }

        public void CalculateLoss(int userId, MonthAndYear monthAndYear)
        {
            var (startSum, monthlyChange, endSum) = _lossRepository.GetMonthlyValues(userId, monthAndYear);

            var loss = Math.Round(monthlyChange + startSum - endSum, 2);

            _lossRepository.UpdateLoss(userId, monthAndYear, loss);
        }
    }
}
