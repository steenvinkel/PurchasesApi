using Business.Models;
using Legacy.Models;
using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacyPostingQueryRepository
    {
        List<LegacyMonthlyTypeSumWithColorAndName> GetMonthlyStatus(int userId, int year, int month);
        (object, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>>) Summary(int userId);
        double GetMonthlyChange(int userId, MonthAndYear monthAndYear);
        int GetLossSubCategoryId(int userId);

        Dictionary<MonthAndYear, IncomeAndExpenses> GetMonthlyIncomeAndExpenses(int userId);
    }
}
