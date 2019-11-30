using Business.Models;
using Legacy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services
{
    public class LegacySumupService : ILegacySumupService
    {
        private readonly ILegacyPostingSumupService _postingSumupService;
        private readonly ILegacyAccountStatusSumupService _accountStatusSumupService;

        public LegacySumupService(ILegacyPostingSumupService postingSumupService, ILegacyAccountStatusSumupService accountStatusSumupService)
        {
            _postingSumupService = postingSumupService;
            _accountStatusSumupService = accountStatusSumupService;
        }

        public List<LegacyMonthlySumup> Sumup(int userId)
        {
            var sumup = _postingSumupService.GetSumup(userId);

            var accountStatusSums = _accountStatusSumupService.GetSumup(userId);

            foreach (var monthlySumup in sumup)
            {
                var monthAndYear = new MonthAndYear(monthlySumup.Year, monthlySumup.Month);

                accountStatusSums.TryGetValue(monthAndYear, out var statusSums);

                var invest = statusSums?.Invest ?? 0;
                var lastMonthSummedFortune = statusSums?.LastMonthSummedFortune ?? 0;
                var monthsWithoutPay = CalculateMonthsWithoutPay(lastMonthSummedFortune, monthlySumup.ExpensesLastYear);

                monthlySumup.Invest = Math.Round(invest, 2);
                monthlySumup.MonthsWithoutPay = Math.Round(monthsWithoutPay, 2);
            }

            return sumup;
        }

        private double CalculateMonthsWithoutPay(double summedFortune, double averageExpenses)
        {
            return averageExpenses == 0
                ? 0
                : summedFortune / averageExpenses;
        }
    }
}
