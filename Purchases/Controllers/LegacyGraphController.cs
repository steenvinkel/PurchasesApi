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