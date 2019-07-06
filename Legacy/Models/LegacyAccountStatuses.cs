using System;

namespace Legacy.Models
{
    public class LegacyAccountStatus
    {
        public int Account_id { get; }
        public int Account_status_id { get; }
        public double Amount { get; }
        public int Year { get; }
        public int Month { get; }

        public LegacyAccountStatus(int account_id, int account_status_id, double amount, int year, int month)
        {
            Account_id = account_id;
            Account_status_id = account_status_id;
            Amount = amount;
            Year = year;
            Month = month;
        }

        public override bool Equals(object obj)
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
