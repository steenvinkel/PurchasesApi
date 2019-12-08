using System.Collections.Generic;
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
            TryGetValue(monthAndYear, out var categories);
            double currentMonthsOwnPension = 0;
            categories?.TryGetValue(Pension, out currentMonthsOwnPension);

            TryGetValue(monthAndYear.PreviousMonth(), out var lastMonthCategories);
            double previousMonthsOwnPension = 0;
            lastMonthCategories?.TryGetValue(Pension, out previousMonthsOwnPension);

            return currentMonthsOwnPension - previousMonthsOwnPension;
        }
    }
}
