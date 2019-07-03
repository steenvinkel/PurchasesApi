using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                           Sum = g.Sum(x => x.posting.Amount)
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
                           Sum = g.Sum(x => x.posting.Amount)
                       };

            var result = inTypes.Union(outTypes).ToList();

            return Ok(result);
        }
    }
}