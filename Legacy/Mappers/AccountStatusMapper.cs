using Business.Models;
using Legacy.Models;
using System.Collections.Generic;
using System.Linq;

namespace Legacy.Mappers
{
    public class AccountStatusMapper
    {
        public object Map(List<AccountStatus> accountStatuses)
        {
            var legacyAccountStatuses = accountStatuses.Select(accountStatus => new LegacyAccountStatuses(
                accountStatus.AccountId,
                accountStatus.AccountStatusId,
                accountStatus.Amount,
                accountStatus.Date.Year,
                accountStatus.Date.Month
            ));

            var yearMonthGroups =
                from accountStatus in legacyAccountStatuses
                group accountStatus by accountStatus.Year into groupYear
                from groupMonth in
                    (from accountStatus in groupYear
                     group accountStatus by accountStatus.Month)
                group groupMonth by groupYear.Key;

            var map = yearMonthGroups.ToDictionary(group => group.Key, group => group.ToDictionary(g => g.Key, g => g.ToList()));

            return map;
        }
    }
}
