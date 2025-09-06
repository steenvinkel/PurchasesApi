using Business.Models;
using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacyPostingQueryRepository
    {
        decimal GetMonthlyChange(int userId, MonthAndYear monthAndYear);
        Dictionary<MonthAndYear, IncomeAndExpenses> GetMonthlyIncomeAndExpenses(int userId);
    }
}
