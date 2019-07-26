using Business.Customizations;
using Business.Models;
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
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Type).ToList();

            var (_, summary) = _postingQueryRepository.Summary(userId);

            var monthlyValues = new Dictionary<(int, int), (double pureInWithoutPension, double pureOut)>();
            var monthly = new List<LegacyMonthlySumup>();
            foreach (var yearData in monthlyResults.GroupBy(x => x.Year))
            {
                var year = yearData.Key;
                foreach (var data in yearData.GroupBy(x => x.Month))
                {
                    var month = data.Key;
                    var monthAndYear = new MonthAndYear(year, month);

                    var @in = data.SingleOrDefault(x => x.Type == "in")?.Sum ?? 0;
                    var @out = data.SingleOrDefault(x => x.Type == "out")?.Sum ?? 0;
                    var tax = data.SingleOrDefault(x => x.Type == "tax")?.Sum ?? 0;

                    double extra = UserSpecifics.CreateExtraLine(userId, year, month, @in, tax);

                    var pureIn = @in - tax;
                    var pureOut = @out - tax;
                    var pensionRate = UserSpecifics.GetPensionRate(userId, monthAndYear);
                    var selfPaidPension = GetSelfPaidPension(summary, year, month, pensionRate);
                    var pureInWithoutPension = @in - selfPaidPension - tax;
                    if (pureInWithoutPension < 0)
                    {
                        pureInWithoutPension = 0;
                    }

                    monthlyValues.Add((year, month), (pureInWithoutPension, pureOut));

                    var savingProcentage = CalculateSavingsRate(pureIn, pureOut);
                    var savingsWithoutPension = CalculateSavingsRate(pureInWithoutPension, pureOut);
                    var savingsLastYear = SavingsRateLastYear(year, month, monthlyValues);

                    var expensesLastYear = AverageExpensesLastYear(monthAndYear, monthlyValues);

                    monthly.Add(new LegacyMonthlySumup
                    {
                        Year = year,
                        Month = month,
                        In = Math.Round(@in, 2),
                        Out = Math.Round(@out, 2),
                        PureOut = Math.Round(pureOut, 2),
                        Savings = Math.Round(savingProcentage, 2),
                        SavingsWithoutOwnContribution = Math.Round(savingsWithoutPension, 2),
                        SavingsLastYear = Math.Round(savingsLastYear, 2),
                        ExpensesLastYear = Math.Round(expensesLastYear, 2),
                        Extra = Math.Round(extra, 2),
                    });
                }
            }

            return monthly;
        }

        private double AverageExpensesLastYear(MonthAndYear month, Dictionary<(int, int), (double pureInWithoutPension, double pureOut)> monthlyValues)
        {
            var count = 0;
            var sumOut = 0.0;

            var months = GetAllMonthsLastYear(month);
            foreach (var current in months)
            {
                if (monthlyValues.ContainsKey((current.Year, current.Month)))
                {
                    var (_, pureOut) = monthlyValues[(current.Year, current.Month)];
                    var @out = pureOut;
                    sumOut += @out;
                    count += 1;
                }
            }

            return count == 0
                ? 0
                : sumOut / count;
        }

        private List<MonthAndYear> GetAllMonthsLastYear(MonthAndYear monthAndYear)
        {
            var months = new List<MonthAndYear>();
            var current = monthAndYear;
            for (int i = 0; i < 12; i++)
            {
                current = current.PreviousMonth();
                months.Add(current);
            }

            return months;
        }

        private double SavingsRateLastYear(int year, int month, Dictionary<(int, int), (double pureInWithoutPension, double pureOut)> monthlyValues)
        {
            var sumIn = 0.0;
            var sumOut = 0.0;
            var currentMonth = month - 1;
            var currentYear = year;
            for (int i = 0; i < 12; i++) {
                if (currentMonth == 0) {
                    currentMonth = 12;
                    currentYear -= 1;
                }

                if (monthlyValues.ContainsKey((currentYear, currentMonth)))
                {
                    var (pureInWithoutPension, pureOut) = monthlyValues[(currentYear, currentMonth)];
                    var @in = pureInWithoutPension;
                    var @out = pureOut;
                    sumIn += @in;
                    sumOut += @out;
                }
  
                currentMonth -= 1;
            }

            return CalculateSavingsRate(sumIn, sumOut);
        }

        private double CalculateSavingsRate(double @in, double @out)
        {
            return @in <= 0
                ? 0
                : (@in - @out) / @in * 100;
        }

        private double GetSelfPaidPension(Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>> summary, int year, int month, double pensionRate)
        {
            if (summary.ContainsKey(2)
                && summary[2].ContainsKey(2)
                && summary[2][2].ContainsKey(year)
                && summary[2][2][year].ContainsKey(month)
                )
            {
                return pensionRate * summary[2][2][year][month];
            }

            return 0;
        }
    }
}
