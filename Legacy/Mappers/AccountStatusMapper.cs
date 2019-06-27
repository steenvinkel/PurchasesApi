using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy.Mappers
{
    public class AccountStatusMapper
    {
        public object Map(List<AccountStatus> accountStatuses)
        {
            var yearMonthGroups =
                from accountStatus in accountStatuses
                group accountStatus by accountStatus.Date.Year into groupYear
                from groupMonth in
                    (from accountStatus in groupYear
                     group accountStatus by accountStatus.Date.Month)
                group groupMonth by groupYear.Key;

            var map = yearMonthGroups.ToDictionary(group => group.Key, group => group.ToDictionary(g => g.Key, g => g.ToList()));

            return map;
        }
    }
}
