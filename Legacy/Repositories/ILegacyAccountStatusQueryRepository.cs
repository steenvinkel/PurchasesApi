using Business.Models;

namespace Legacy.Repositories
{
    public interface ILegacyAccountStatusQueryRepository
    {
        (dynamic, dynamic) MonthlyAccountStatus(int userId);
        MonthlyAccountCategorySums GetAccumulatedCategorySums(int userId);
        (double StartSum, double EndSum) StartAndEndOfMonthAccountStatusSum(int userId, MonthAndYear monthAndYear);
    }
}
