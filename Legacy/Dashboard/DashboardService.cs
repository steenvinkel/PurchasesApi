﻿using Business.Models;
using Business.Services;
using Legacy.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Legacy.Dashboard
{
    public class DashboardService(ILegacyPostingQueryRepository postingQueryRepository, ILegacyAccountStatusQueryRepository monthlyAccountStatusRepository) : IDashboardService
    {
        private readonly ILegacyPostingQueryRepository _postingQueryRepository = postingQueryRepository;
        private readonly ILegacyAccountStatusQueryRepository _monthlyAccountStatusRepository = monthlyAccountStatusRepository;

        public Dictionary<int, Dictionary<string, DashboardInformation>> GetDashboards(int userId, List<int> monthsInDashboard, bool allMonthAndYears, List<decimal> returnRates, int currentAge, int pensionAge)
        {
            var monthlyIncomeAndExpenses = _postingQueryRepository.GetMonthlyIncomeAndExpenses(userId);
            var monthlyAccountCategorySums = _monthlyAccountStatusRepository.GetAccumulatedCategorySums(userId);

            var monthlyLedgers = CreateMonthlyLedgers(monthlyIncomeAndExpenses, monthlyAccountCategorySums);

            var monthAndYears = allMonthAndYears 
                ? monthlyIncomeAndExpenses.Select(x => x.Key) 
                : new List<MonthAndYear> { MonthAndYear.Now };

            var dashboards = monthsInDashboard.ToDictionary(numMonths => numMonths, numMonths => monthAndYears.ToDictionary(monthAndYear => monthAndYear.ToString(), monthAndYear =>
                    CalculateDashboard(monthAndYear, returnRates, currentAge, pensionAge, numMonths, monthlyLedgers)));

            return dashboards;
        }

        private static DashboardInformation CalculateDashboard(MonthAndYear currentMonthAndYear, List<decimal> returnRates, int currentAge, int pensionAge, int numMonths, Dictionary<MonthAndYear, Ledger> monthlyLedgers)
        {
            var ledger = CalculateAverageLedger(monthlyLedgers, numMonths, currentMonthAndYear);
            return CalculateDashboard(ledger, currentAge, pensionAge, returnRates);
        }

        private static DashboardInformation CalculateDashboard(Ledger ledger, int currentAge, int pensionAge, List<decimal> returnRates)
        {
            var yearToPension = pensionAge - currentAge;

            var fireAges = returnRates
                .ToDictionary(returnRate => returnRate, returnRate =>
                Calculator.FireAge(ledger.Income, ledger.Expenses, ledger.Fortune, returnRate, currentAge, pensionAge));

            var dashboardInformation = new DashboardInformation
            {
                MonthsLivableWithoutPay = Calculator.CalculateMonthsLivableWithoutPay(ledger.Fortune, ledger.Expenses),
                SavingsRate = Calculator.SavingsRate(ledger.Income, ledger.Expenses),
                Fortune = ledger.Fortune,
                InvestedFortune = ledger.InvestedFortune,
                FireAgePerReturnRate = fireAges,
                Income = ledger.Income,
                Expenses = ledger.Expenses,
                NetworthIncreaseFortune = ledger.FortuneIncrease,
                NetworthIncreaseInvestment = ledger.InvestmentIncrease,
                VariableExpenses = ledger.VariableExpenses,
                FixedExpenses = ledger.FixedExpenses
            };

            return dashboardInformation;
        }

        private static Ledger CalculateAverageLedger(Dictionary<MonthAndYear, Ledger> monthlyLedgers, int numMonths, MonthAndYear currentMonthAndYear)
        {
            return CalculateAverageLedgers(monthlyLedgers, numMonths)[currentMonthAndYear];
        }

        private static Dictionary<MonthAndYear, Ledger> CalculateAverageLedgers(Dictionary<MonthAndYear, Ledger> monthlyLedgers, int numMonths)
        {
            if (numMonths == 0)
            {
                return monthlyLedgers;
            }
            return monthlyLedgers.ToDictionary(pair => pair.Key, pair  =>
            {
                var monthAndYear = pair.Key;
                var values = pair.Value;
                var previousMonthAndYears = monthAndYear.PreviousMonths(numMonths);

                var previousMonths = previousMonthAndYears
                    .Where(month => monthlyLedgers.ContainsKey(month))
                    .Select(month => monthlyLedgers[month])
                    .DefaultIfEmpty(new Ledger());
                monthlyLedgers.TryGetValue(monthAndYear.PreviousMonth(), out var previousMonthlyLedger);
                return new Ledger
                {
                    Income = previousMonths.Average(x => x.Income),
                    Expenses = previousMonths.Average(x => x.Expenses),
                    VariableExpenses = previousMonths.Average(x => x.VariableExpenses),
                    FixedExpenses = previousMonths.Average(x => x.FixedExpenses),
                    Fortune = previousMonthlyLedger?.Fortune ?? 0,
                    InvestedFortune = previousMonthlyLedger?.InvestedFortune ?? 0,
                    FortuneIncrease = previousMonths.Average(x => x.FortuneIncrease),
                    InvestmentIncrease = previousMonths.Average(x => x.InvestmentIncrease)
                };
            });
        }

        private static Dictionary<MonthAndYear, Ledger> CreateMonthlyLedgers(Dictionary<MonthAndYear, IncomeAndExpenses> monthlyIncomeAndExpenses, MonthlyAccountCategorySums monthlyAccountCategorySums)
        {
            var monthlyLedgers = new Dictionary<MonthAndYear, Ledger>();
            foreach (var pair in monthlyIncomeAndExpenses)
            {
                var monthAndYear = pair.Key;

                var income = pair.Value.Income;
                var expenses = pair.Value.Expenses;
                var variableExpenses = pair.Value.VariableExpenses;
                var fixedExpenses = pair.Value.FixedExpenses;
                var summedFortunes = monthlyAccountCategorySums.GetCombinedFortune(monthAndYear);
                var investedFortune = monthlyAccountCategorySums.GetInvestedFortune(monthAndYear);
                var values = new Ledger
                {
                    Income = income,
                    Expenses = expenses,
                    VariableExpenses = variableExpenses,
                    FixedExpenses = fixedExpenses,
                    Fortune = summedFortunes,
                    InvestedFortune = investedFortune,
                    FortuneIncrease = monthlyAccountCategorySums.GetFortune(monthAndYear),
                    InvestmentIncrease = monthlyAccountCategorySums.GetInvestment(monthAndYear),
                };

                monthlyLedgers.Add(monthAndYear, values);
            }

            return monthlyLedgers;
        }
    }
}
