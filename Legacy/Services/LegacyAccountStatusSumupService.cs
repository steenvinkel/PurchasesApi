using System;
using System.Collections.Generic;
using Business.Models;
using Business.Repositories;
using Legacy.Models;
using Legacy.Repositories;
using System.Linq;

namespace Legacy.Services
{
    public class LegacyAccountStatusSumupService : ILegacyAccountStatusSumupService
    {
        private readonly ILegacyAccountStatusQueryRepository _monthlyAccountStatusRepository;
        private readonly IAccountStatusRepository _accountStatusRepository;

        public LegacyAccountStatusSumupService(ILegacyAccountStatusQueryRepository monthlyAccountStatusRepository, IAccountStatusRepository accountStatusRepository)
        {
            _monthlyAccountStatusRepository = monthlyAccountStatusRepository;
            _accountStatusRepository = accountStatusRepository;
        }

        public Dictionary<MonthAndYear, LegacyAccountStatusSums> GetSumup(int userId)
        {
            var monthlyInvestmentMap = _accountStatusRepository.Get(userId)
                .GroupBy(accountStatus => new MonthAndYear(accountStatus.Date.Year, accountStatus.Date.Month), x => x.Amount)
                .ToDictionary(x => x.Key, x => Math.Round(x.Sum() * 0.02 / 12, 2));

            Dictionary<MonthAndYear, double> summedFortunes = _monthlyAccountStatusRepository.CalculateSummedFortunes(userId);

            var allKeys = monthlyInvestmentMap.Keys.Union(summedFortunes.Keys).Distinct().OrderBy(x => x).ToList();

            // Remove first account status month, since there is no account statuses previous to this
            allKeys.RemoveAt(0);

            var map = new Dictionary<MonthAndYear, LegacyAccountStatusSums>();
            foreach(var monthAndYear in allKeys)
            {
                var sums = new LegacyAccountStatusSums
                {
                    Invest = Math.Round(monthlyInvestmentMap[monthAndYear], 2),
                    LastMonthSummedFortune = summedFortunes[monthAndYear.PreviousMonth()]
                };

                map.Add(monthAndYear, sums);
            }

            return map;
        }
    }
}
