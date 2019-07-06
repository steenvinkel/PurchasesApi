using Legacy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Repositories
{
    public interface ILegacyAccountStatusRepository
    {
        List<LegacyAccountStatus> Post(List<LegacyAccountStatus> accountStatuses, int userId);
        List<LegacyAccountStatus> Put(List<LegacyAccountStatus> accountStatuses, int userId);
    }
}
