using Business.Models;
using DataAccess.Models;
using Legacy.Repositories;
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

        public MonthlyAccountCategorySums GetAccumulatedCategorySums(int userId)
        {
            var monthlyAccountStatuses = MonthlyAccountStatus(userId);

            var accountCategories = new MonthlyAccountCategorySums(monthlyAccountStatuses);

            return accountCategories;
        }

        private Dictionary<MonthAndYear, Dictionary<string, decimal>> MonthlyAccountStatus(int userId)
        {
            var categoryStatuses =
                (from mas in (from account in _context.Account
                              join accountStatus in _context.AccountStatus on account.AccountId equals accountStatus.AccountId
                              join accumulatedCategory in _context.AccumulatedCategory on account.AccumulatedCategoryId equals accumulatedCategory.AccumulatedCategoryId
                              where account.UserId == userId
                              select new { accountStatus.Date.Year, accountStatus.Date.Month, accountStatus.Amount, accumulatedCategory.Name })
                 group mas by new { mas.Year, mas.Month, mas.Name } into g
                 select new {
                     MonthAndYear = new MonthAndYear(g.Key.Year, g.Key.Month),
                     g.Key.Name,
                     Sum = g.Sum(x => x.Amount)
                 })
                 .ToList()
                 .GroupBy(x => x.MonthAndYear)
                 .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Name, y => y.Sum));

            return categoryStatuses;
        }

        public (decimal StartSum, decimal EndSum) StartAndEndOfMonthAccountStatusSum(int userId, MonthAndYear monthAndYear)
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
