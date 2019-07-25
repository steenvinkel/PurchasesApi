using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models
{
    public class MonthAndYear
    {
        public int Year { get; }
        public int Month { get; }

        public MonthAndYear(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public MonthAndYear PreviousMonth()
        {
            var lastMonth = Month - 1;

            return new MonthAndYear(lastMonth == 0 ? Year - 1 : Year, lastMonth == 0 ? 12 : lastMonth);
        }

        public bool IsEarlierThan(MonthAndYear other)
        {
            return Year < other.Year || (Month <= other.Month && Year == other.Year);
        }

        public DateTime LastDayOfMonth()
        {
            return new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));
        }

        public override bool Equals(object obj)
        {
            return obj is MonthAndYear year &&
                   Year == year.Year &&
                   Month == year.Month;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Month);
        }
    }
}
