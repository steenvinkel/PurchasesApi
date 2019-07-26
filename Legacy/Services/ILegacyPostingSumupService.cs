using Legacy.Models;
using System.Collections.Generic;

namespace Legacy.Services
{
    public interface ILegacyPostingSumupService
    {
        List<LegacyMonthlySumup> GetSumup(int userId);
    }
}
