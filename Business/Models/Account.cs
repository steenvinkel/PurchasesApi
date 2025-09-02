using System.Collections.Generic;

namespace Business.Models
{
    public class Account(int accountId, string name, string purpose, List<AccountStatus> Statuses)
    {
        public int AccountId { get; private set; } = accountId;
        public string Name { get; private set; } = name;
        public string Purpose { get; private set; } = purpose;
        public List<AccountStatus> Statuses { get; set; } = Statuses;
    }
}
