using System.Collections.Generic;

namespace Legacy.Dashboard
{
    public interface IDashboardService
    {
        Dictionary<int,Dictionary<string, DashboardInformation>> GetDashboards(int userId, List<int> monthsInDashboard, bool allMonthAndYears, List<double> returnRates, int currentAge, int pensionAge);
    }
}