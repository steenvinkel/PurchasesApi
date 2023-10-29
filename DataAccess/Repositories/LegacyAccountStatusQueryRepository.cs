using Business.Models;
using DataAccess.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacyMonthlyAccountStatusRepository : ILegacyAccountStatusQueryRepository
    {
        private readonly PurchasesContext _context;

        public LegacyMonthlyAccountStatusRepository(PurchasesContext context)
        {
            _context = context;
        }

        public (dynamic, dynamic) MonthlyAccountStatus(int userId)
        {
            var categories = _context.AccumulatedCategory.Where(ac => ac.UserId == userId).Select(ac => new { Id = ac.AccumulatedCategoryId, ac.Color, ac.Name }).ToList();

            var categoryStatuses =
                (from mas in (from account in _context.Account
                      join accountStatus in _context.AccountStatus on account.AccountId equals accountStatus.AccountId
                      join accumulatedCategory in _context.AccumulatedCategory on account.AccumulatedCategoryId equals accumulatedCategory.AccumulatedCategoryId
                      where account.UserId == userId
                      select new { accountStatus.Date.Year, accountStatus.Date.Month, account.AccumulatedCategoryId, accountStatus.Amount })
                 group mas by new { mas.Year, mas.Month, mas.AccumulatedCategoryId } into g
                 select new { g.Key.Year, g.Key.Month, g.Key.AccumulatedCategoryId, Sum = g.Sum(x => x.Amount) })
                 .ToList();

            var status = categoryStatuses.GroupBy(x => new { x.Year, x.Month }).Select(x => new {
                x.Key.Year,
                x.Key.Month,
                Sum = Math.Round(x.Sum(y => y.Sum), 2),
                Categories = categories.ToDictionary(c => c.Name, c => new
                {
                    Amount = Math.Round(x.SingleOrDefault(y => y.AccumulatedCategoryId == c.Id)?.Sum ?? 0, 2),
                    c.Color,
                    c.Name
                })
            }).ToList();

            return (categories, status);
        }

        public MonthlyAccountCategorySums GetAccumulatedCategorySums(int userId)
        {
            var (_, monthlyAccountStatuses) = MonthlyAccountStatus(userId);

            var accountCategories = new MonthlyAccountCategorySums();
            foreach (var status in monthlyAccountStatuses)
            {
                var dictionary = new Dictionary<string, double>();
                accountCategories.Add(new MonthAndYear((int)status.Year, (int)status.Month), dictionary);

                foreach (var category in status.Categories.Values)
                {
                    dictionary.Add(category.Name, category.Amount);
                }
            }

            return accountCategories;
        }

        public (double StartSum, double EndSum) StartAndEndOfMonthAccountStatusSum(int userId, MonthAndYear monthAndYear)
        {
            var monthlyAccountStatuses = _context.AccountStatus
                .Where(accountStatus => accountStatus.UserId == userId)
                .GroupBy(accountStatus => new { accountStatus.Date.Year, accountStatus.Date.Month })
                .Select(pair => new { pair.Key.Year, pair.Key.Month, Sum = pair.Sum(x => x.Amount) })
                .ToDictionary(x => new MonthAndYear(x.Year, x.Month), x => x.Sum);

            var endSum = monthlyAccountStatuses[monthAndYear];
            var startSum = monthlyAccountStatuses[monthAndYear.PreviousMonth()];

            return (startSum, endSum);
        }
    }
}
