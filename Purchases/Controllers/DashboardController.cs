using System.Collections.Generic;
using System.Linq;
using Legacy.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchases.Helpers;

namespace Purchases.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public ActionResult GetDashboardInformation([FromQuery] List<int> monthsInDashboard, [FromQuery] List<decimal> returnRates, bool allMonthAndYears, int currentAge, int pensionAge)
        {
            var userId = HttpContext.GetUserId();

            if (!monthsInDashboard.Any())
            {
                monthsInDashboard = new List<int> { 0, 1, 3, 6, 12 };
            }
            if (!returnRates.Any())
            {
                returnRates = new List<decimal> { 0, 0.05m };
            }

            var dashboards = _dashboardService.GetDashboards(userId, monthsInDashboard, allMonthAndYears, returnRates, currentAge, pensionAge);

            return Ok(dashboards);
        }
    }
}