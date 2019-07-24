using Business.Models;
using DataAccess.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacyMonthlyAccountStatusRepository : ILegacyMonthlyAccountStatusRepository
    {
        private readonly PurchasesContext _context;

        public LegacyMonthlyAccountStatusRepository(PurchasesContext context)
        {
            _context = context;
        }

        public (dynamic, dynamic) MonthlyAccountStatus(int userId)
        {
            var categories = _context.AccumulatedCategory.Where(ac => ac.UserId == userId).Select(ac => new { Id = ac.AccumulatedCategoryId, ac.Color, ac.Name }).ToList();
            var categoriesMap = categories.ToDictionary(c => c.Id);

            var categoryStatuses = from mas in (from account in _context.Account
                            join accountStatus in _context.AccountStatus on account.AccountId equals accountStatus.AccountId
                            join accumulatedCategory in _context.AccumulatedCategory on account.AccumulatedCategoryId equals accumulatedCategory.AccumulatedCategoryId
                            where account.UserId == userId
                            select new { accountStatus.Date.Year, accountStatus.Date.Month, account.AccumulatedCategoryId, accountStatus.Amount })
                         group mas by new { mas.Year, mas.Month, mas.AccumulatedCategoryId } into g
                         select new { g.Key.Year, g.Key.Month, g.Key.AccumulatedCategoryId, Sum = g.Sum(x => x.Amount) }
                            ;

            var status = categoryStatuses.ToList().GroupBy(x => new { x.Year, x.Month }).Select(x => new {
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

        public Dictionary<MonthAndYear, double> CalculateSummedFortunes(int userId)
        {
            var (_, monthlyAccountStatuses) = MonthlyAccountStatus(userId);
            var summedFortunes = new Dictionary<MonthAndYear, double>();
            foreach (var status in monthlyAccountStatuses)
            {
                var amount = 0.0;
                foreach (var pair in status.Categories)
                {
                    var category = pair.Value;
                    if (category.Name == "Fortune" || category.Name == "Investment")
                    {
                        amount += category.Amount;
                    }
                }
                summedFortunes.Add(new MonthAndYear((int)status.Year, (int)status.Month), amount);
            }

            return summedFortunes;
        }
    }
}
