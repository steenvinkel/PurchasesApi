using Legacy.Models;
using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacySumupRepository
    {
        List<MonthlyTypeSum> Sumup(int userId);
    }
}
