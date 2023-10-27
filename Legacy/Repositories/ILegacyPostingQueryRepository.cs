using Business.Models;
using Legacy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Repositories
{
    public interface ILegacyPostingQueryRepository
    {
        List<LegacyDailyNum> GetDailyPurchases(int userId);
        List<LegacyMonthlyTypeSumWithColorAndName> GetMonthlyStatus(int userId, int year, int month);
        List<MonthlyTypeSum> Sumup(int userId);
        (object, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>>) Summary(int userId);
        Dictionary<MonthAndYear, double> GetSubcategoryMonthlySum(int userId, int subcategoryId);
        double GetMonthlyChange(int userId, MonthAndYear monthAndYear);
        int GetLossSubCategoryId(int userId);

        Dictionary<MonthAndYear, IncomeAndExpenses> GetMonthlyIncomeAndExpenses(int userId);
    }
}
