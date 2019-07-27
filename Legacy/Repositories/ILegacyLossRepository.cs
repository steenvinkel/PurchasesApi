using Business.Models;

namespace Legacy.Repositories
{
    public interface ILegacyLossRepository
    {
        void UpdateLoss(int userId, MonthAndYear monthAndYear, double loss);
    }
}
