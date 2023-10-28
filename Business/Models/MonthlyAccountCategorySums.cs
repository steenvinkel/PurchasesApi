using System.Collections.Generic;
using System.Linq;

namespace Business.Models
{
    public class MonthlyAccountCategorySums : Dictionary<MonthAndYear, Dictionary<string, double>>
    {
        public const string Fortune = "Fortune";
        public const string Investment = "Investment";

        public MonthlyAccountCategorySums(IDictionary<MonthAndYear, Dictionary<string, double>> dictionary) : base(dictionary) {}
        public MonthlyAccountCategorySums() {}

        public double GetCombinedFortune(MonthAndYear monthAndYear)
        {
            TryGetValue(monthAndYear, out var categories);

            return categories?.Select(pair => pair.Value).Sum() ?? 0;
        }

        public double GetFortune(MonthAndYear monthAndYear)
        {
            TryGetValue(monthAndYear, out var categories);
            double currentMonthsFortune = 0;
            categories?.TryGetValue(Fortune, out currentMonthsFortune);

            TryGetValue(monthAndYear.PreviousMonth(), out var lastMonthCategories);
            double previousMonthsFortune = 0;
            lastMonthCategories?.TryGetValue(Fortune, out previousMonthsFortune);

            return currentMonthsFortune - previousMonthsFortune;
        }

        public double GetInvestment(MonthAndYear monthAndYear)
        {
            TryGetValue(monthAndYear, out var categories);
            double currentMonthsInvestment = 0;
            categories?.TryGetValue(Investment, out currentMonthsInvestment);

            TryGetValue(monthAndYear.PreviousMonth(), out var lastMonthCategories);
            double previousMonthsInvestment = 0;
            lastMonthCategories?.TryGetValue(Investment, out previousMonthsInvestment);

            return currentMonthsInvestment - previousMonthsInvestment;
        }
    }
}
