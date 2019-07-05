using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacyMonthlyAccountStatusRepository
    {
        (dynamic, dynamic) MonthlyAccountStatus(int userId);
        Dictionary<(int, int), double> CalculateSummedFortunes(int userId);
    }
}
