using System.Linq;
using Business.Repositories;
using Legacy.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegacyGraphController : ControllerBase
    {
        // Postings
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ILegacyPostingQueryRepository _postingQueryRepository;

        public LegacyGraphController(ILegacyPostingQueryRepository graphRepository, ISubCategoryRepository subCategoryRepository)
        {
            _postingQueryRepository = graphRepository;
            _subCategoryRepository = subCategoryRepository;
        }

        [HttpGet("Summary")]
        public ActionResult Summary()
        {
            var userId = HttpContext.GetUserId();

            var (categories, summary) = _postingQueryRepository.Summary(userId);

            return Ok(new
            {
                categories,
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

        [HttpGet("MonthlyStatus")]
        public ActionResult Get(int year, int month)
        {
            var userId = HttpContext.GetUserId();

            var monthlyStatuses = _postingQueryRepository.GetMonthlyStatus(userId, year, month);

            return Ok(monthlyStatuses);
        }
    }
}