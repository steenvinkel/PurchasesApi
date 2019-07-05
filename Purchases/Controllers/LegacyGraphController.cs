using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Models;
using Legacy.Repositories;
using Legacy.Services;
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
        private readonly ILegacySumupService _sumupService;
        private readonly ILegacySummaryRepository _summaryRepository;
        private readonly ILegacyMonthlyAccountStatusRepository _monthlyAccountStatusRepository;

        public LegacyGraphController(PurchasesContext context, ILegacySumupService sumupService, ILegacySummaryRepository summaryRepository, ILegacyMonthlyAccountStatusRepository monthlyAccountStatusRepository)
        {
            _context = context;
            _sumupService = sumupService;
            _summaryRepository = summaryRepository;
            _monthlyAccountStatusRepository = monthlyAccountStatusRepository;
        }

        [HttpGet("Sumup")]
        public ActionResult Sumup()
        {
            var userId = HttpContext.GetUserId();

            var sumup = _sumupService.Sumup(userId);

            return Ok(sumup);
        }

        [HttpGet("Summary")]
        public ActionResult Summary()
        {
            var userId = HttpContext.GetUserId();

            var (categories, subcategories, summary) = _summaryRepository.Summary(userId);

            return Ok(new
            {
                categories,
                subcategories,
                summary
            });
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

            var (categories, status) = _monthlyAccountStatusRepository.MonthlyAccountStatus(userId);

            return Ok(new
            {
                Categories = categories,
                status = status
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