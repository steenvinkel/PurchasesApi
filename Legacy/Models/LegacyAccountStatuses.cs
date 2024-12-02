using System;

namespace Legacy.Models
{
    public class LegacyAccountStatus(int account_id, int account_status_id, double amount, int year, int month)
    {
        public int Account_id { get; } = account_id;
        public int Account_status_id { get; } = account_status_id;
        public double Amount { get; } = amount;
        public int Year { get; } = year;
        public int Month { get; } = month;

        public override bool Equals(object? obj)
        {
            return obj is LegacyAccountStatus other &&
                   Account_id == other.Account_id &&
                   Account_status_id == other.Account_status_id &&
                   Amount == other.Amount &&
                   Year == other.Year &&
                   Month == other.Month;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Account_id, Account_status_id, Amount, Year, Month);
        }
    }
}
