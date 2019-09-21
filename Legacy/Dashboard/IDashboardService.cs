using System.Collections.Generic;

namespace Legacy.Dashboard
{
    public interface IDashboardService
    {
        Dictionary<int, DashboardInformation> GetDashboards(int userId, List<int> monthsInDashboard, List<double> returnRates, int currentAge, int pensionAge);
    }
}