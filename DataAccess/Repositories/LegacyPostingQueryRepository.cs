using Business.Constants;
using Business.Models;
using DataAccess.Models;
using Legacy.Models;
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

        public List<LegacyMonthlyTypeSumWithColorAndName> GetMonthlyStatus(int userId, int year, int month)
        {
            var inTypes =
                       (from posting in _context.PostingForUser(userId)
                        join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                        join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                        where category.Type == CategoryProperties.Type.In
                             && posting.Date.Year == year && posting.Date.Month == month
                        select new { posting, category, subcategory }
                             )
                            .ToList()
                            .GroupBy((x) => new { x.posting.Date.Year, x.posting.Date.Month, x.subcategory.SubcategoryId })
                            .Select(g => new
                                {
                                    g.Key.Year,
                                    g.Key.Month,
                                    g.First().category.Type,
                                    g.First().subcategory.Name,
                                    g.First().subcategory.Color,
                                    Sum = Math.Round(g.Sum(x => x.posting.Amount), 2)
                                });

            var outTypes =
                       (from posting in _context.PostingForUser(userId)
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where category.Type == CategoryProperties.Type.Out
                            && posting.Date.Year == year && posting.Date.Month == month
                        select new { posting, category }
                             )
                            .ToList()
                            .GroupBy((x) => new { x.posting.Date.Year, x.posting.Date.Month, x.category.CategoryId })
                            .Select(g => new
                                {
                                    g.Key.Year,
                                    g.Key.Month,
                                    g.First().category.Type,
                                    g.First().category.Name,
                                    g.First().category.Color,
                                    Sum = Math.Round(g.Sum(x => x.posting.Amount), 2)
                                });

            var result = inTypes.Union(outTypes).Select(x => new LegacyMonthlyTypeSumWithColorAndName
            {
                Year = x.Year,
                Month = x.Month,
                Type = x.Type,
                Name = x.Name,
                Color = x.Color,
                Sum = x.Sum
            }).ToList();

            return result;
        }

        public (object, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>>) Summary(int userId)
        {
            var categories = _context.CategoryForUser(userId)
                .Select(c => new { c.Name, Category_id = c.CategoryId.ToString() })
                .ToList();

            var summary =
                (from posting in _context.PostingForUser(userId)
                 join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                 join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                 group posting.Amount by new { posting.Date.Year, posting.Date.Month, subcategory.SubcategoryId, category.CategoryId } into g
                 select new {
                     g.Key.Year,
                     g.Key.Month,
                     g.Key.SubcategoryId,
                     g.Key.CategoryId,
                     Sum = g.Sum()
                 } ).ToList();

            var summaryMap = summary
                .GroupBy(s => s.CategoryId).ToDictionary(gc => gc.Key,
                    gc => gc.GroupBy(s => s.SubcategoryId).ToDictionary(gs => gs.Key,
                        gs => gs.GroupBy(s => s.Year).ToDictionary(gy => gy.Key,
                            gy => gy.GroupBy(s => s.Month).ToDictionary(gm => gm.Key, gm => gm.Sum(x => x.Sum)
                            )
                        )
                    )
                );

            return (categories, summaryMap);
        }

        public double GetMonthlyChange(int userId, MonthAndYear monthAndYear)
        {
            return (from P in _context.PostingForUser(userId)
                    join S in _context.Subcategory on P.SubcategoryId equals S.SubcategoryId
                    join C in _context.Category on S.CategoryId equals C.CategoryId
                    where  P.Date.Year == monthAndYear.Year
                        && P.Date.Month == monthAndYear.Month
                        && C.Name != CategoryProperties.Name.Loss
                    select C.Type == CategoryProperties.Type.In ? P.Amount : -P.Amount)
                                .Sum();
        }

        public Dictionary<MonthAndYear, IncomeAndExpenses> GetMonthlyIncomeAndExpenses(int userId)
        {
            var incomeAndExpenses =
                (from p in _context.PostingForUser(userId)
                 join s in _context.Subcategory on p.SubcategoryId equals s.SubcategoryId
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
