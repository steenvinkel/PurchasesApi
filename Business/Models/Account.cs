using System.Collections.Generic;

namespace Business.Models
{
    public class Account(int accountId, string name, string accumulatedCategoryName, List<AccountStatus> Statuses)
    {
        public int AccountId { get; private set; } = accountId;
        public string Name { get; private set; } = name;
        public string AccumulatedCategoryName { get; private set; } = accumulatedCategoryName;
        public List<AccountStatus> Statuses { get; set; } = Statuses;
    }
}
