using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Legacy.Services
{
    public class LegacySumupService : ILegacySumupService
    {
        private readonly ILegacySumupRepository _sumupRepository;
        private readonly ILegacySummaryRepository _summaryRepository;
        private readonly ILegacyMonthlyAccountStatusRepository _monthlyAccountStatusRepository;

        public LegacySumupService(ILegacySumupRepository sumupRepository, ILegacySummaryRepository summaryRepository, ILegacyMonthlyAccountStatusRepository monthlyAccountStatusRepository)
        {
            _sumupRepository = sumupRepository;
            _summaryRepository = summaryRepository;
            _monthlyAccountStatusRepository = monthlyAccountStatusRepository;
        }

        public List<dynamic> Sumup(int userId)
        {
            var monthlyResults = _sumupRepository.Sumup(userId)
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Type).ToList();

            // The first month is always useless since it only contains the account status
            monthlyResults.RemoveFirstMonthInFirstYear();

            var (_, __, summary) = _summaryRepository.Summary(userId);

            Dictionary<(int, int), double> summedFortunes = _monthlyAccountStatusRepository.CalculateSummedFortunes(userId);

            var monthlyValues = new Dictionary<(int, int), (double pureInWithoutPension, double pureOut)>();
            var monthly = new List<dynamic>();
            foreach (var yearData in monthlyResults.GroupBy(x => x.Year))
            {
                var year = yearData.Key;
                foreach (var data in yearData.GroupBy(x => x.Month))
                {
                    var month = data.Key;
                    var @in = data.SingleOrDefault(x => x.Type == "in")?.Sum ?? 0;
                    var @out = data.SingleOrDefault(x => x.Type == "out")?.Sum ?? 0;
                    var invest = data.SingleOrDefault(x => x.Type == "invest")?.Sum ?? 0;
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

                    var expensesLastYear = AverageExpensesLastYear(year, month, monthlyValues);
                    var monthsWithoutPay = GetMonthsLivableWithoutPay(year, month, summedFortunes, expensesLastYear);


                    monthly.Add(new
                    {
                        year,
                        month,
                        In = (double)@in,
                        Out = (double)@out,
                        pureOut = (double)pureOut,
                        invest = (double)invest,
                        savings = (double)savingProcentage,
                        savingsWithoutOwnContribution = (double)savingsWithoutPension,
                        savingsLastYear = (double)savingsLastYear,
                        expensesLastYear = (double)expensesLastYear,
                        monthsWithoutPay = (double)monthsWithoutPay,
                        extra = (double)extra,
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

        private (int, int) GetLastMonth(int year, int month)
        {
            var lastMonth = month - 1;

            return (lastMonth == 0 ? year - 1 : year, lastMonth == 0 ? 12 : lastMonth);
        }


        private double GetMonthsLivableWithoutPay(int year, int month, Dictionary<(int, int), double> summedFortune, double expensesLastYear)
        {
            var lastPeriod = GetLastMonth(year, month);
            return expensesLastYear == 0
                ? 0
                : summedFortune[lastPeriod] / expensesLastYear;
        }

        private double AverageExpensesLastYear(int year, int month, Dictionary<(int, int), (double pureInWithoutPension, double pureOut)> monthlyValues)
        {
            var count = 0;
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
                    var values = monthlyValues[(currentYear, currentMonth)];
                    var @out = values.pureOut;
                    sumOut += @out;
                    count += 1;
                }
  
                currentMonth -= 1;
            }

            return count == 0
                ? 0
                : sumOut / count;
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
                    var values = monthlyValues[(currentYear, currentMonth)];
                    var @in = values.pureInWithoutPension;
                    var @out = values.pureOut;
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
                : (@in - @out) / @in *100;
        }


        private double GetSelfPaidPension(Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>> summary, int year, int month)
        {
            if (year < 2016 || (month <= 9 && year == 2016)) {
                return 0;
            }
            if (summary.ContainsKey(2)
                && summary[2].ContainsKey(2)
                && summary[2][2].ContainsKey(year)
                && summary[2][2][year].ContainsKey(month)
                )
            {

                var pay = summary[2][2][year][month];
  
                var pensionRate = 0.0625;
                if (year < 2018 || (month <= 7 && year == 2018)) {
                    pensionRate = 0.0525;
                }
                var pension = Math.Round(pay * pensionRate, 2);

                return pension;
            }

            return 0;
        }

    }
}
