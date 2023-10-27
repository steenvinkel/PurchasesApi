using System.Linq;
using Business.Repositories;
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
        // Accounts
        private readonly ILegacyAccountStatusQueryRepository _monthlyAccountStatusRepository;

        // Postings
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ILegacyPostingQueryRepository _postingQueryRepository;

        // Mixed
        private readonly ILegacySumupService _sumupService;

        public LegacyGraphController(ILegacySumupService sumupService, ILegacyAccountStatusQueryRepository monthlyAccountStatusRepository, ILegacyPostingQueryRepository graphRepository, ISubCategoryRepository subCategoryRepository)
        {
            _sumupService = sumupService;
            _monthlyAccountStatusRepository = monthlyAccountStatusRepository;
            _postingQueryRepository = graphRepository;
            _subCategoryRepository = subCategoryRepository;
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

            var subcategories = _subCategoryRepository.GetList(userId)
                .Select(sc => new { sc.Name, Subcategory_id = sc.Id, Category_id = sc.CategoryId } )
                .GroupBy(x => x.Category_id)
                .ToDictionary(x => x.Key);

            var (categories, summary) = _postingQueryRepository.Summary(userId);

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

            var dailyPurchases = _postingQueryRepository.GetDailyPurchases(userId);

            return Ok(dailyPurchases);
        }

        [HttpGet("MonthlyAccountStatus")]
        public ActionResult Get()
        {
            var userId = HttpContext.GetUserId();

            var (categories, status) = _monthlyAccountStatusRepository.MonthlyAccountStatus(userId);

            return Ok(new
            {
                Categories = categories,
                status
            });
        }

        [HttpGet("MonthlyStatus")]
        public ActionResult Get(int year, int month)
        {
            var userId = HttpContext.GetUserId();

            var monthlyStatuses = _postingQueryRepository.GetMonthlyStatus(userId, year, month);

            return Ok(monthlyStatuses);
        }
    }
}