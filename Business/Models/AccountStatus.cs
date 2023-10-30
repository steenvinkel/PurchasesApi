using System;

namespace Business.Models
{
    public class AccountStatus
    {
        public AccountStatus(int accountStatusId, int accountId, DateTime date, double amount)
        {
            AccountStatusId = accountStatusId;
            AccountId = accountId;
            Date = date;
            Amount = amount;
        }

        public int AccountStatusId { get; private set; }
        public int AccountId { get; private set; }
        public DateTime Date { get; private set; }
        public double Amount { get; private set; }
    }
}
