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
        private readonly ILegacySumupRepository _sumupRepository;
        private readonly ILegacySummaryRepository _summaryRepository;

        public LegacyPostingSumupService(ILegacySumupRepository sumupRepository, ILegacySummaryRepository summaryRepository)
        {
            _sumupRepository = sumupRepository;
            _summaryRepository = summaryRepository;
        }

        public List<LegacyMonthlySumup> GetSumup(int userId)
        {
            var monthlyResults = _sumupRepository.Sumup(userId)
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Type).ToList();

            var (_, summary) = _summaryRepository.Summary(userId);

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

                    double extra = CreateExtraLine(userId, year, month, @in, tax);

                    var pureIn = @in - tax;
                    var pureOut = @out - tax;
                    var selfPaidPension = GetSelfPaidPension(summary, year, month);
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

        private static double CreateExtraLine(int userId, int year, int month, double @in, double tax)
        {
            var extra = 0.0;
            if (userId == 1)
            {
                var procent50 = 19590.0;
                if (year > 2015 || (month > 6 && year == 2015))
                {
                    procent50 = 16800.0 + 10600;
                }
                if (year > 2016 || (month > 9 && year == 2016))
                {
                    procent50 = 19200.0 + 13900;
                }
                if (year > 2016 || (month > 11 && year == 2016))
                {
                    procent50 = 16000.0 + 26000;
                }
                if (year > 2017 || (month > 11 && year == 2017))
                {
                    procent50 = (@in - tax) / 2 + tax;
                }
                extra = procent50;
            }

            return extra;
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

        private double GetSelfPaidPension(Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>> summary, int year, int month)
        {
            if (summary.ContainsKey(2)
                && summary[2].ContainsKey(2)
                && summary[2][2].ContainsKey(year)
                && summary[2][2][year].ContainsKey(month)
                )
            {
                return CalculatePension(year, month, summary[2][2][year][month]);
            }

            return 0;
        }

        private double CalculatePension(int year, int month, double pay)
        {
            var pensionRate = 0.0625;
            if (year < 2018 || (month <= 7 && year == 2018)) {
                pensionRate = 0.0525;
            }
            if (year < 2016 || (month <= 9 && year == 2016)) {
                return 0;
            }
            var pension = Math.Round(pay * pensionRate, 2);

            return pension;
        }
    }
}
