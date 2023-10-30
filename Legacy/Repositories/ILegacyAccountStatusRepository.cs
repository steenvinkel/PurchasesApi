using Legacy.Models;
using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacyAccountStatusRepository
    {
        List<LegacyAccountStatus> Post(List<LegacyAccountStatus> accountStatuses, int userId);
        List<LegacyAccountStatus> Put(List<LegacyAccountStatus> accountStatuses, int userId);
    }
}
