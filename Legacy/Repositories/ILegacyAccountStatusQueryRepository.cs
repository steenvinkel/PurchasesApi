using Business.Models;

namespace Legacy.Repositories
{
    public interface ILegacyAccountStatusQueryRepository
    {
        MonthlyAccountCategorySums GetAccumulatedCategorySums(int userId);
        (decimal StartSum, decimal EndSum) StartAndEndOfMonthAccountStatusSum(int userId, MonthAndYear monthAndYear);
    }
}
