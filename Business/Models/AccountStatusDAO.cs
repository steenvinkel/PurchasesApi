using System;

namespace Business.Models
{
    public class AccountStatusDAO(int accountStatusId, int accountId, DateTime date, decimal amount)
    {
        public int AccountStatusId { get; private set; } = accountStatusId;
        public int AccountId { get; private set; } = accountId;
        public DateTime Date { get; private set; } = date;
        public decimal Amount { get; private set; } = amount;
    }
}
