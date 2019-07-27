using Business.Constants;
using Business.Customizations;
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

        public List<LegacyDailyNum> GetDailyPurchases(int userId)
        {
            var dailyPurchases = 
                (from posting in _context.Posting
                join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                where posting.UserId == userId && category.Type == CategoryProperties.Type.Out
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
            var taxCategoryId = UserSpecifics.GetTaxCategoryId(userId);

            var sums = from posting in _context.Posting
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where category.Type == CategoryProperties.Type.Out && category.CategoryId != taxCategoryId && posting.UserId == userId
                       group posting by new { posting.Date.Year, posting.Date.Month } into g
                       select new
                       {
                           g.Key.Year,
                           g.Key.Month,
                           Sum = g.Sum(x => x.Amount)
                       };

            var sumPerDay = sums.ToList().Select(sum => new LegacyMonthlySumPerDay
            {
                Year = sum.Year,
                Month = sum.Month,
                SumPerDay = Math.Round(sum.Sum / DateTime.DaysInMonth(sum.Year, sum.Month), 2)
            });

            return sumPerDay.ToList();
        }

        public List<LegacyMonthlyTypeSumWithColorAndName> GetMonthlyStatus(int userId, int year, int month)
        {
            var inTypes =
                       from posting in _context.Posting
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where posting.UserId == userId && category.Type == CategoryProperties.Type.In
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
                       from posting in _context.Posting
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where posting.UserId == userId && category.Type == CategoryProperties.Type.Out
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
            var taxCategoryId = UserSpecifics.GetTaxCategoryId(userId);

            var inAndOut = (from p in _context.Posting
                            join s in _context.Subcategory on p.SubcategoryId equals s.SubcategoryId
                            join c in _context.Category on s.CategoryId equals c.CategoryId
                            where p.UserId == userId
                            group p.Amount by new { p.Date.Year, p.Date.Month, c.Type } into g
                            select new
                            {
                                g.Key.Year,
                                g.Key.Month,
                                g.Key.Type,
                                Sum = g.Sum()
                            }).ToList();

            var tax = (from p in _context.Posting
                       join s in _context.Subcategory on p.SubcategoryId equals s.SubcategoryId
                       join c in _context.Category on s.CategoryId equals c.CategoryId
                       where p.UserId == userId && c.Type == CategoryProperties.Type.Out && c.CategoryId == taxCategoryId
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
            var categories = _context.Category
                .Where(c => c.UserId == userId)
                .Select(c => new { c.Name, Category_id = c.CategoryId.ToString() })
                .ToList();

            var summary =
                (from posting in _context.Posting
                 join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                 join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                 where posting.UserId == userId
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
            var monthlySum = _context.Posting
                .Where(posting => posting.UserId == userId)
                .Where(posting => posting.SubcategoryId == subcategoryId)
                .GroupBy(posting => new { posting.Date.Year, posting.Date.Month })
                .Select(pair => new {pair.Key, Sum = pair.Sum(x => x.Amount)})
                .ToDictionary(a => new MonthAndYear(a.Key.Year, a.Key.Month), a => a.Sum);

            return monthlySum;
        }
    }
}
