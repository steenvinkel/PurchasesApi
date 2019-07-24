using Legacy.Models;
using System.Collections.Generic;

namespace Legacy.Services
{
    public interface ILegacySumupService
    {
        List<LegacyMonthlySumup> Sumup(int userId);
    }
}
