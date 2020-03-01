using Business.Constants;
using Business.Customizations;
using Business.Models;
using DataAccess.Models;
using Legacy.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Repositories
{
    public class LegacyPostingQueryRepository : ILegacyPostingQueryRepository
    {
        private readonly PurchasesContext _context;

        public LegacyPostingQueryRepository(PurchasesContext context)
        {
            _context = context;
        }

        private Dictionary<MonthAndYear, Dictionary<int, double>> GetMonthlySubcategorySum(int userId, Expression<Func<Category, bool>> categoryFilter, Expression<Func<Subcategory, bool>> subcategoryFilter)
        {
            var sums = (from P in _context.PostingForUser(userId)
                        join S in _context.Subcategory.Where(subcategoryFilter) on P.SubcategoryId equals S.SubcategoryId
                        join C in _context.Category.Where(categoryFilter) on S.CategoryId equals C.CategoryId
                        group P by new { P.Date.Year, P.Date.Month, S.SubcategoryId } into g
                        select new
                        {
                            MonthAndYear = new MonthAndYear(g.Key.Year, g.Key.Month),
                            g.Key.SubcategoryId,
                            Sum = g.Sum(posting => posting.Amount)
                        }).ToList();

            return sums.GroupBy(x => x.MonthAndYear).ToDictionary(x => x.Key, x => x.ToDictionary(y => y.SubcategoryId, y => y.Sum));
        }

        public List<LegacyDailyNum> GetDailyPurchases(int userId)
        {
            var dailyPurchases = 
                (from posting in _context.PostingForUser(userId)
                join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                where category.Type == CategoryProperties.Type.Out
                 group posting by new { posting.Date.Year, posting.Date.Month, posting.Date.Day } into g
                select new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    Num = g.Count()
                }).ToList().Select(dp => new LegacyDailyNum
                {
                    Year = dp.Year,
                    Month = dp.Month,
                    Day = dp.Day,
                    NumPurchases = dp.Num
                }).ToList();

            dailyPurchases.RemoveAll(p => UserSpecifics.ShouldDailyPurchaseMonthBeRemoved(new MonthAndYear(p.Year, p.Month), userId));

            return dailyPurchases.ToList();
        }

        public List<LegacyMonthlySumPerDay> GetMonthlyAverageDailyPurchases(int userId)
        {
            var taxSubcategoryIds = UserSpecifics.GetTaxSubcategoryIds(userId);

            var sums = GetMonthlySubcategorySum(userId, 
                category => category.Type == CategoryProperties.Type.Out,
                subcategory => !taxSubcategoryIds.Contains(subcategory.SubcategoryId))
                .Select(x => new LegacyMonthlySumPerDay
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    SumPerDay = Math.Round(x.Value.Values.Sum() / DateTime.DaysInMonth(x.Key.Year, x.Key.Month), 2)
                })
                .ToList();
            return sums;
        }

        public List<LegacyMonthlyTypeSumWithColorAndName> GetMonthlyStatus(int userId, int year, int month)
        {
            var inTypes =
                       from posting in _context.PostingForUser(userId)
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where category.Type == CategoryProperties.Type.In
                            && posting.Date.Year == year && posting.Date.Month == month
                       group new { posting, category, subcategory } by new { posting.Date.Year, posting.Date.Month, subcategory.SubcategoryId } into g
                       select new {
                           g.Key.Year,
                           g.Key.Month,
                           g.First().category.Type,
                           g.First().subcategory.Name,
                           g.First().subcategory.Color,
                           Sum = Math.Round(g.Sum(x => x.posting.Amount), 2)
                       };

            var outTypes =
                       from posting in _context.PostingForUser(userId)
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where category.Type == CategoryProperties.Type.Out
                            && posting.Date.Year == year && posting.Date.Month == month
                       group new { posting, category } by new { posting.Date.Year, posting.Date.Month, category.CategoryId } into g
                       select new {
                           g.Key.Year,
                           g.Key.Month,
                           g.First().category.Type,
                           g.First().category.Name,
                           g.First().category.Color,
                           Sum = Math.Round(g.Sum(x => x.posting.Amount), 2)
                       };

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

        public List<MonthlyTypeSum> Sumup(int userId)
        {
            var taxSubcategoryIds = UserSpecifics.GetTaxSubcategoryIds(userId);

            var inAndOut = (from p in _context.PostingForUser(userId)
                            join s in _context.Subcategory on p.SubcategoryId equals s.SubcategoryId
                            join c in _context.Category on s.CategoryId equals c.CategoryId
                            group p.Amount by new { p.Date.Year, p.Date.Month, c.Type } into g
                            select new
                            {
                                g.Key.Year,
                                g.Key.Month,
                                g.Key.Type,
                                Sum = g.Sum()
                            }).ToList();

            var tax = (from p in _context.PostingForUser(userId)
                       join s in _context.Subcategory on p.SubcategoryId equals s.SubcategoryId
                       join c in _context.Category on s.CategoryId equals c.CategoryId
                       where c.Type == CategoryProperties.Type.Out
                            && taxSubcategoryIds.Contains(s.SubcategoryId)
                       group p.Amount by new { p.Date.Year, p.Date.Month } into g
                       select new
                       {
                           g.Key.Year,
                           g.Key.Month,
                           Type = "tax",
                           Sum = g.Sum()
                       }
                ).ToList();

            return inAndOut.Union(tax).Select(x => new MonthlyTypeSum
            {
                MonthAndYear = (x.Year, x.Month),
                Type = x.Type,
                Sum = x.Sum,
            }).ToList();
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

        public Dictionary<MonthAndYear, double> GetSubcategoryMonthlySum(int userId, int subcategoryId)
        {
            var monthlySum = _context.PostingForUser(userId)
                .Where(posting => posting.SubcategoryId == subcategoryId)
                .GroupBy(posting => new { posting.Date.Year, posting.Date.Month })
                .Select(pair => new {pair.Key, Sum = pair.Sum(x => x.Amount)})
                .ToDictionary(a => new MonthAndYear(a.Key.Year, a.Key.Month), a => a.Sum);

            return monthlySum;
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

        public int GetLossSubCategoryId(int userId)
        {
            return (from S in _context.Subcategory
                    join C in _context.CategoryForUser(userId) on S.CategoryId equals C.CategoryId
                    where C.Name == CategoryProperties.Name.Loss
                    select S.SubcategoryId).Single();
        }

        public Dictionary<MonthAndYear, IncomeExpensesAndTax> GetMonthlyIncomeExpenseAndTax(int userId)
        {
            var monthlyTypeSums = Sumup(userId);

            var monthlyIncomeExpensesAndTaxes = new Dictionary<MonthAndYear, IncomeExpensesAndTax>();
            foreach(var typeSumsGroupedMonthly in monthlyTypeSums.GroupBy(x => x.MonthAndYear))
            {
                var incomeExpensesAndTax = new IncomeExpensesAndTax
                {
                    Income = typeSumsGroupedMonthly.SingleOrDefault(x => x.Type == "in")?.Sum ?? 0,
                    Expenses = typeSumsGroupedMonthly.SingleOrDefault(x => x.Type == "out")?.Sum ?? 0,
                    Tax = typeSumsGroupedMonthly.SingleOrDefault(x => x.Type == "tax")?.Sum ?? 0,
                };
                monthlyIncomeExpensesAndTaxes.Add(typeSumsGroupedMonthly.Key, incomeExpensesAndTax);
            }
            return monthlyIncomeExpensesAndTaxes;
        }
    }
}
