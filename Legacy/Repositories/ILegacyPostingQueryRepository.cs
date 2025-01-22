using Business.Models;
using Legacy.Models;
using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacyPostingQueryRepository
    {
        List<LegacyMonthlyTypeSumWithColorAndName> GetMonthlyStatus(int userId, int year, int month);
        (object, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, decimal>>>>) Summary(int userId);
        decimal GetMonthlyChange(int userId, MonthAndYear monthAndYear);
        Dictionary<MonthAndYear, IncomeAndExpenses> GetMonthlyIncomeAndExpenses(int userId);
    }
}
