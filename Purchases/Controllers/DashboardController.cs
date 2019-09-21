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
        public ActionResult GetDashboardInformation([FromQuery] List<int> monthsInDashboard, [FromQuery] List<double> returnRates, int currentAge, int pensionAge)
        {
            var userId = HttpContext.GetUserId();

            if (!monthsInDashboard.Any())
            {
                monthsInDashboard = new List<int> { 0, 1, 3, 6, 12 };
            }
            if (!returnRates.Any())
            {
                returnRates = new List<double> { 0, 0.05 };
            }

            var dashboards = _dashboardService.GetDashboards(userId, monthsInDashboard, returnRates, currentAge, pensionAge);

            return Ok(dashboards);
        }
    }
}