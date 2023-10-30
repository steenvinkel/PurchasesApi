using Business.Models;

namespace Legacy.Repositories
{
    public interface ILegacyAccountStatusQueryRepository
    {
        MonthlyAccountCategorySums GetAccumulatedCategorySums(int userId);
        (double StartSum, double EndSum) StartAndEndOfMonthAccountStatusSum(int userId, MonthAndYear monthAndYear);
    }
}
