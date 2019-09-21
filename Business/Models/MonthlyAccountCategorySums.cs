﻿using System.Collections.Generic;
using System.Linq;

namespace Business.Models
{
    public class MonthlyAccountCategorySums : Dictionary<MonthAndYear, Dictionary<string, double>>
    {
        public const string Pension = "Pension";
        public const string Fortune = "Fortune";
        public const string Investment = "Investment";

        public MonthlyAccountCategorySums(IDictionary<MonthAndYear, Dictionary<string, double>> dictionary) : base(dictionary) {}
        public MonthlyAccountCategorySums() {}

        public double GetFortunesWithoutPension(MonthAndYear monthAndYear)
        {
            TryGetValue(monthAndYear, out var categories);

            return categories?.Where(pair => pair.Key != Pension).Select(pair => pair.Value).Sum() ?? 0;
        }

        public double GetOwnPensionSavings(MonthAndYear monthAndYear)
        {
            var currentMonthsOwnPension = this[monthAndYear][Pension];

            TryGetValue(monthAndYear.PreviousMonth(), out var categories);
            var previousMonthsOwnPension = categories?[Pension] ?? 0;

            return currentMonthsOwnPension - previousMonthsOwnPension;
        }
    }
}
