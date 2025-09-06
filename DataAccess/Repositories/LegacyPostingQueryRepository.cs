using Business.Constants;
using DataAccess.Constants;
using Business.Models;
using DataAccess.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacyPostingQueryRepository : ILegacyPostingQueryRepository
    {
        private readonly PurchasesContext _context;

        public LegacyPostingQueryRepository(PurchasesContext context)
        {
            _context = context;
        }

        public decimal GetMonthlyChange(int userId, MonthAndYear monthAndYear)
        {
            return (from P in _context.PostingForUser(userId)
                    join S in _context.SubCategory on P.SubcategoryId equals S.SubcategoryId
                    join C in _context.Category on S.CategoryId equals C.CategoryId
                    where  P.Date.Year == monthAndYear.Year
                        && P.Date.Month == monthAndYear.Month
                        && S.Name != Business.Constants.SubCategoryProperties.Name.Loss
                    select C.Type == CategoryProperties.Type.In ? P.Amount : -P.Amount)
                                .Sum();
        }

        public Dictionary<MonthAndYear, IncomeAndExpenses> GetMonthlyIncomeAndExpenses(int userId)
        {
            var incomeAndExpenses =
                (from p in _context.PostingForUser(userId)
                 join s in _context.SubCategory on p.SubcategoryId equals s.SubcategoryId
                 join c in _context.Category on s.CategoryId equals c.CategoryId
                 group p.Amount by new { p.Date.Year, p.Date.Month, c.Type, SubType = s.Type } into g
                 select new
                 {
                     MonthAndYear = new MonthAndYear(g.Key.Year, g.Key.Month),
                     g.Key.Type,
                     g.Key.SubType,
                     Sum = g.Sum()
                 })
                    .ToList()
                    .GroupBy(x => x.MonthAndYear)
                    .ToDictionary(x => x.Key, x => new IncomeAndExpenses
                    {
                        Income = x.Where(y => y.Type == "in").Sum(y => y.Sum),
                        Expenses = x.Where(y => y.Type == "out").Sum(y => y.Sum),
                        VariableExpenses = x.Where(y => y.SubType == "variable").Sum(y => y.Sum),
                        FixedExpenses = x.Where(y => y.SubType == "fixed").Sum(y => y.Sum),
                    });

            return incomeAndExpenses;
        }
    }
}
