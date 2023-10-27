using Business;
using Business.Customizations;
using Business.Models;
using Legacy.ExtensionMethods;
using Legacy.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Legacy.Services
{
    public class LegacyPostingSumupService : ILegacyPostingSumupService
    {
        private readonly ILegacyPostingQueryRepository _postingQueryRepository;

        public LegacyPostingSumupService(ILegacyPostingQueryRepository postingQueryRepository)
        {
            _postingQueryRepository = postingQueryRepository;
        }

        public List<LegacyMonthlySumup> GetSumup(int userId)
        {
            var monthlyResults = _postingQueryRepository.Sumup(userId)
                .OrderBy(x => x.MonthAndYear.Year).ThenBy(x => x.MonthAndYear.Month).ThenBy(x => x.Type).ToList();

            var monthlyValues = new Dictionary<MonthAndYear, (double pureInWithoutPension, double pureOut)>();
            var monthly = new List<LegacyMonthlySumup>();
            foreach (var data in monthlyResults.GroupBy(x => x.MonthAndYear))
            {
                var monthAndYear = data.Key;

                var @in = data.SingleOrDefault(x => x.Type == "in")?.Sum ?? 0;
                var @out = data.SingleOrDefault(x => x.Type == "out")?.Sum ?? 0;
                var tax = data.SingleOrDefault(x => x.Type == "tax")?.Sum ?? 0;

                double extra = UserSpecifics.CreateExtraLine(userId, monthAndYear, @in, tax);

                var pureIn = @in - tax;
                var pureOut = @out - tax;

                monthlyValues.Add(monthAndYear, (pureIn, pureOut));

                var savingProcentage = Calculate.SavingsRate(pureIn, pureOut);
                var savingsLastYear = SavingsRateLastYear(monthAndYear, monthlyValues);

                var expensesLastYear = AverageExpensesLastYear(monthAndYear, monthlyValues);

                monthly.Add(new LegacyMonthlySumup
                {
                    Year = monthAndYear.Year,
                    Month = monthAndYear.Month,
                    In = Math.Round(@in, 2),
                    Out = Math.Round(@out, 2),
                    PureOut = Math.Round(pureOut, 2),
                    Savings = Math.Round(savingProcentage, 2),
                    SavingsLastYear = Math.Round(savingsLastYear, 2),
                    ExpensesLastYear = Math.Round(expensesLastYear, 2),
                    Extra = Math.Round(extra, 2),
                });
            }

            return monthly;
        }

        private double AverageExpensesLastYear(MonthAndYear month, Dictionary<MonthAndYear, (double pureIn, double pureOut)> monthlyValues)
        {
            var averageExpensesLastYear = GetAllMonthsLastYear(month)
                .Where(monthAndYear => monthlyValues.ContainsKey(monthAndYear))
                .Select(monthAndYear => monthlyValues[monthAndYear].pureOut)
                .DefaultIfEmpty(0)
                .Average();

            return averageExpensesLastYear;
        }

        private double SavingsRateLastYear(MonthAndYear monthAndYear, Dictionary<MonthAndYear, (double pureIn, double pureOut)> monthlyValues)
        {
            var savingsRateLastYear = GetAllMonthsLastYear(monthAndYear)
                .Where(month => monthlyValues.ContainsKey(month))
                .Select(month => monthlyValues[month])
                .Select(values => (In: values.pureIn, Out: values.pureOut))
                .Aggregate((In: 0.0, Out: 0.0), (sum, values) => (In: sum.In + values.In, Out: sum.Out + values.Out))
                .CalculateSavingsRate();

            return savingsRateLastYear;
        }

        private List<MonthAndYear> GetPreviousMonths(MonthAndYear monthAndYear, int numberOfMonths)
        {
            var months = new List<MonthAndYear>();
            var current = monthAndYear;
            for (int i = 0; i < numberOfMonths; i++)
            {
                current = current.PreviousMonth();
                months.Add(current);
            }

            return months;
        }

        private List<MonthAndYear> GetAllMonthsLastYear(MonthAndYear monthAndYear)
        {
            return GetPreviousMonths(monthAndYear, 12);
        }
    }
}
