using Legacy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Repositories
{
    public interface ILegacyGraphRepository
    {
        List<LegacyDailyNum> GetDailyPurchases(int userId);
        List<LegacyMonthlySumPerDay> GetMonthlyAverageDailyPurchases(int userId);
        List<LegacyMonthlyTypeSumWithColorAndName> GetMonthlyStatus(int userId, int year, int month);
    }
}
