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
        private readonly ILegacyPostingQueryRepository _postingQueryRepository;

        public LegacyGraphController(ILegacyPostingQueryRepository graphRepository)
        {
            _postingQueryRepository = graphRepository;
        }

        [HttpGet("Summary")]
        public ActionResult Summary()
        {
            var userId = HttpContext.GetUserId();

            var summary = _postingQueryRepository.Summary(userId);

            return Ok(new
            {
                summary
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