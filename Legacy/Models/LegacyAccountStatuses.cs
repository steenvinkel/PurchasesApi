using System;

namespace Legacy.Models
{
    public class LegacyAccountStatuses
    {
        public int AccountId { get; }
        public int AccountStatusId { get; }
        public double Amount { get; }
        public int Year { get; }
        public int Month { get; }

        public LegacyAccountStatuses(int accountId, int accountStatusId, double amount, int year, int month)
        {
            AccountId = accountId;
            AccountStatusId = accountStatusId;
            Amount = amount;
            Year = year;
            Month = month;
        }

        public override bool Equals(object obj)
        {
            return obj is LegacyAccountStatuses other &&
                   AccountId == other.AccountId &&
                   AccountStatusId == other.AccountStatusId &&
                   Amount == other.Amount &&
                   Year == other.Year &&
                   Month == other.Month;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AccountId, AccountStatusId, Amount, Year, Month);
        }
    }
}
