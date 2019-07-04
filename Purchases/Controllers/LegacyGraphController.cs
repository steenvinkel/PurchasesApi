using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyGraphController : ControllerBase
    {
        private readonly PurchasesContext _context;

        public LegacyGraphController(PurchasesContext context)
        {
            _context = context;
        }

        [HttpGet("Summary")]
        public ActionResult Summary()
        {
            var userId = HttpContext.GetUserId();

            var categories = _context.Category
                .Where(c => c.UserId == userId)
                .Select(c => new { c.Name, CategoryId = c.CategoryId.ToString() })
                .ToList();

            var subCategories = _context.Subcategory
                .Join(_context.Category, s => s.CategoryId, c => c.CategoryId, (s, c) => new { s, c.UserId })
                .Where(r => r.UserId == userId)
                .Select(pair => new
                {
                    pair.s.Name,
                    SubcategoryId = pair.s.SubcategoryId.ToString(),
                    CategoryId = pair.s.CategoryId.ToString(),
                })
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

            return Ok(summaryMap);
        }

        [HttpGet("NumDailyPurchases")]
        public ActionResult NumDailyPurchases()
        {
            var userId = HttpContext.GetUserId();

            var dailyPurcases = 
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

            dailyPurcases.RemoveAll(p => p.Year == 2014 && p.Month < 9);

            return Ok(dailyPurcases);
        }

        [HttpGet("SumPerDay")]
        public ActionResult SumPerDay()
        {
            var userId = HttpContext.GetUserId();
            var sums = from posting in _context.Posting
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where category.Type == "out" && category.CategoryId != 15 && posting.UserId == userId
                       group posting by new { posting.Date.Year, posting.Date.Month } into g
                       select new
                       {
                           g.Key.Year,
                           g.Key.Month,
                           Sum = g.Sum(x => x.Amount)
                       };

            var sumPerDay = sums.ToList().Select(sum => new
            {
                Year = sum.Year.ToString(),
                Month = sum.Month.ToString(),
                SumPerDay = Math.Round(sum.Sum / DateTime.DaysInMonth(sum.Year, sum.Month), 2)
            });

            return Ok(sumPerDay);
        }

        [HttpGet("MonthlyAccountStatus")]
        public ActionResult Get()
        {
            var userId = HttpContext.GetUserId();
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
                Year = x.Key.Year.ToString(),
                Month = x.Key.Month.ToString(),
                Sum = Math.Round(x.Sum(y => y.Sum), 2),
                Categories = categories.ToDictionary(c => c.Name, c => new
                {
                    Amount = Math.Round(x.SingleOrDefault(y => y.AccumulatedCategoryId == c.Id)?.Sum ?? 0, 2),
                    c.Color,
                    c.Name
                })
            });

            return Ok(new
            {
                Categories = categories,
                Status = status
            });
        }

        [HttpGet("MonthlyStatus")]
        public ActionResult Get(int year, int month)
        {
            var userId = HttpContext.GetUserId();

            var inTypes =
                       from posting in _context.Posting
                       join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                       join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                       where posting.UserId == userId && category.Type == "in"
                            && posting.Date.Year == year && posting.Date.Month == month
                       group new { posting, category, subcategory } by new { posting.Date.Year, posting.Date.Month, subcategory.SubcategoryId } into g
                       select new {
                           Year = g.Key.Year.ToString(),
                           Month = g.Key.Month.ToString(),
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
                           Year = g.Key.Year.ToString(),
                           Month = g.Key.Month.ToString(),
                           g.First().category.Type,
                           g.First().category.Name,
                           g.First().category.Color,
                           Sum = Math.Round(g.Sum(x => x.posting.Amount), 2)
                       };

            var result = inTypes.Union(outTypes).ToList();

            return Ok(result);
        }
    }
}