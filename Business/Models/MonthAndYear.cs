using System;
using System.Collections.Generic;

namespace Business.Models
{
    public class MonthAndYear : IComparable<MonthAndYear>
    {
        public int Year { get; }
        public int Month { get; }

        public static MonthAndYear Now => new(DateTime.Now);

        public MonthAndYear(DateTime dateTime)
        {
            Year = dateTime.Year;
            Month = dateTime.Month;
        }

        public MonthAndYear(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public static implicit operator MonthAndYear((int Year, int Month) pair)
        {
            return new MonthAndYear(pair.Year, pair.Month);
        }

        public List<MonthAndYear> PreviousMonths(int numberOfMonths)
        {
            var months = new List<MonthAndYear>();
            var current = this;
            for (int i = 0; i < numberOfMonths; i++)
            {
                current = current.PreviousMonth();
                months.Add(current);
            }

            return months;
        }

        public MonthAndYear PreviousMonth()
        {
            var lastMonth = Month - 1;

            return new MonthAndYear(lastMonth == 0 ? Year - 1 : Year, lastMonth == 0 ? 12 : lastMonth);
        }

        public MonthAndYear NextMonth()
        {
            var nextMonth = Month + 1;

            return new MonthAndYear(nextMonth == 13 ? Year + 1 : Year, nextMonth == 13 ? 1 : nextMonth);
        }

        public bool IsEarlierThan(MonthAndYear other)
        {
            return Year < other.Year || (Month < other.Month && Year == other.Year);
        }

        public bool IsLaterThan(MonthAndYear other)
        {
            return Year > other.Year || (Month > other.Month && Year == other.Year);
        }

        public DateTime LastDayOfMonth()
        {
            return new DateTime(Year, Month, DateTime.DaysInMonth(Year, Month));
        }

        public override bool Equals(object? obj)
        {
            return obj is MonthAndYear year &&
                   Year == year.Year &&
                   Month == year.Month;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, Month);
        }

        public int CompareTo(MonthAndYear? other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            if (Equals(other))
            {
                return 0;
            }

            return IsEarlierThan(other) ? -1 : 1;
        }

        public override string ToString()
        {
            return Year + "-" + Month.ToString("00");
        }
    }
}
