using Business.Customizations;
using DataAccess.Models;
using Legacy.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacyGraphRepository : ILegacyGraphRepository
    {
        private readonly PurchasesContext _context;

        public LegacyGraphRepository(PurchasesContext context)
        {
            _context = context;
        }

        public List<LegacyDailyNum> GetDailyPurchases(int userId)
        {
            var dailyPurchases = 
                (from posting in _context.Posting
                join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                where posting.UserId == userId && category.Type == "out"
                group posting by new { posting.Date.Year, posting.Date.Month, posting.Date.Day } into g
                select new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    Num = g.Count()
                }).ToList();

            if (Rules.IsJcpSpecific(userId))
            {
                dailyPurchases.RemoveAll(p => p.Year == 2014 && p.Month < 9);
            }

            return dailyPurchases.Select(dp => new LegacyDailyNum
            {
                Year = dp.Year,
                Month = dp.Month,
                Day = dp.Day,
                NumPurchases = dp.Num
            }).ToList();
        }

        public List<LegacyMonthlySumPerDay> GetMonthlyAverageDailyPurchases(int userId)
        {
            var taxCategoryId = UserSpecifics.GetTaxCategoryId(userId);

            var sums = from posting in _context.Posting
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where category.Type == "out" && category.CategoryId != taxCategoryId && posting.UserId == userId
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
                       where posting.UserId == userId && category.Type == "in"
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
                       where posting.UserId == userId && category.Type == "out"
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
    }
}
