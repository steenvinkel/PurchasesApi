using Business.Models;

namespace Legacy.Services
{
    public interface ILegacyLossService
    {
        void CalculateLoss(int userId, MonthAndYear monthAndYear);
    }
}
