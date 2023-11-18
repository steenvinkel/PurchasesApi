using System.Collections.Generic;
using System.Linq;

namespace Business.Models
{
    public class MonthlyAccountCategorySums : Dictionary<MonthAndYear, Dictionary<string, decimal>>
    {
        public const string Fortune = "Fortune";
        public const string Investment = "Investment";

        public MonthlyAccountCategorySums(IDictionary<MonthAndYear, Dictionary<string, decimal>> dictionary) : base(dictionary) {}
        public MonthlyAccountCategorySums() {}

        public decimal GetCombinedFortune(MonthAndYear monthAndYear)
        {
            TryGetValue(monthAndYear, out var categories);

            return categories?.Select(pair => pair.Value).Sum() ?? 0;
        }

        public decimal GetInvestedFortune(MonthAndYear monthAndYear)
        {
            TryGetValue(monthAndYear, out var categories);

            return categories?.Where(pair => pair.Key == Investment).Select(pair => pair.Value).Sum() ?? 0;
        }

        public decimal GetFortune(MonthAndYear monthAndYear)
        {
            TryGetValue(monthAndYear, out var categories);
            decimal currentMonthsFortune = 0;
            categories?.TryGetValue(Fortune, out currentMonthsFortune);

            TryGetValue(monthAndYear.PreviousMonth(), out var lastMonthCategories);
            decimal previousMonthsFortune = 0;
            lastMonthCategories?.TryGetValue(Fortune, out previousMonthsFortune);

            return currentMonthsFortune - previousMonthsFortune;
        }

        public decimal GetInvestment(MonthAndYear monthAndYear)
        {
            TryGetValue(monthAndYear, out var categories);
            decimal currentMonthsInvestment = 0;
            categories?.TryGetValue(Investment, out currentMonthsInvestment);

            TryGetValue(monthAndYear.PreviousMonth(), out var lastMonthCategories);
            decimal previousMonthsInvestment = 0;
            lastMonthCategories?.TryGetValue(Investment, out previousMonthsInvestment);

            return currentMonthsInvestment - previousMonthsInvestment;
        }
    }
}
