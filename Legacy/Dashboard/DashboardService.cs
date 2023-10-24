using Business.Models;
using Business.Services;
using Legacy.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Legacy.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly ILegacyPostingQueryRepository _postingQueryRepository;
        private readonly ILegacyAccountStatusQueryRepository _monthlyAccountStatusRepository;

        public DashboardService(ILegacyPostingQueryRepository postingQueryRepository, ILegacyAccountStatusQueryRepository monthlyAccountStatusRepository)
        {
            _postingQueryRepository = postingQueryRepository;
            _monthlyAccountStatusRepository = monthlyAccountStatusRepository;
        }

        public Dictionary<int, Dictionary<string, DashboardInformation>> GetDashboards(int userId, List<int> monthsInDashboard, bool allMonthAndYears, List<double> returnRates, int currentAge, int pensionAge)
        {
            var monthlyIncomeExpensesAndTax = _postingQueryRepository.GetMonthlyIncomeExpenseAndTax(userId);
            var monthlyAccountCategorySums = _monthlyAccountStatusRepository.GetAccumulatedCategorySums(userId);

            var monthlyLedgers = CreateMonthlyLedgers(monthlyIncomeExpensesAndTax, monthlyAccountCategorySums);

            var monthAndYears = allMonthAndYears 
                ? monthlyIncomeExpensesAndTax.Select(x => x.Key) 
                : new List<MonthAndYear> { MonthAndYear.Now };

            var dashboards = monthsInDashboard.ToDictionary(numMonths => numMonths, numMonths => monthAndYears.ToDictionary(monthAndYear => monthAndYear.ToString(), monthAndYear => 
                    CalculateDashboard(monthAndYear, returnRates, currentAge, pensionAge, numMonths, monthlyLedgers)));

            return dashboards;
        }

        private DashboardInformation CalculateDashboard(MonthAndYear currentMonthAndYear, List<double> returnRates, int currentAge, int pensionAge, int numMonths, Dictionary<MonthAndYear, Ledger> monthlyLedgers)
        {
            var ledger = CalculateAverageLedger(monthlyLedgers, numMonths, currentMonthAndYear);
            return CalculateDashboard(ledger, currentAge, pensionAge, returnRates);
        }

        private DashboardInformation CalculateDashboard(Ledger ledger, int currentAge, int pensionAge, List<double> returnRates)
        {
            var yearToPension = pensionAge - currentAge;
            var calculator = new Calculator();

            var fireAges = returnRates
                .ToDictionary(returnRate => returnRate, returnRate => 
                calculator.FireAge(ledger.IncomeWithoutTaxAndPension, ledger.ExpensesWithoutTax, ledger.Fortune, returnRate, currentAge, pensionAge));

            var dashboardInformation = new DashboardInformation
            {
                MonthsLivableWithoutPay = calculator.CalculateMonthsLivableWithoutPay(ledger.Fortune, ledger.ExpensesWithoutTax),
                SavingsRate = calculator.SavingsRate(ledger.IncomeWithoutTaxAndPension, ledger.ExpensesWithoutTax),
                Fortune = ledger.Fortune,
                FireAgePerReturnRate = fireAges,
                IncomeWithoutTaxAndPension = ledger.IncomeWithoutTaxAndPension,
                ExpensesWithoutTax = ledger.ExpensesWithoutTax,
                NetworthIncreaseFortune = ledger.FortuneIncrease,
                NetworthIncreaseInvestment = ledger.InvestmentIncrease,
                VariableExpenses = ledger.VariableExpenses,
                FixedExpenses = ledger.FixedExpenses
            };

            return dashboardInformation;
        }

        private Ledger CalculateAverageLedger(Dictionary<MonthAndYear, Ledger> monthlyLedgers, int numMonths, MonthAndYear currentMonthAndYear)
        {
            return CalculateAverageLedgers(monthlyLedgers, numMonths)[currentMonthAndYear];
        }

        private Dictionary<MonthAndYear, Ledger> CalculateAverageLedgers(Dictionary<MonthAndYear, Ledger> monthlyLedgers, int numMonths)
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
                    IncomeWithoutTaxAndPension = previousMonths.Average(x => x.IncomeWithoutTaxAndPension),
                    Expenses = previousMonths.Average(x => x.Expenses),
                    ExpensesWithoutTax = previousMonths.Average(x => x.ExpensesWithoutTax),
                    VariableExpenses = previousMonths.Average(x => x.VariableExpenses),
                    FixedExpenses = previousMonths.Average(x => x.FixedExpenses),
                    Tax = previousMonths.Average(x => x.Tax),
                    OwnPension = previousMonths.Average(x => x.OwnPension),
                    Fortune = previousMonthlyLedger?.Fortune ?? 0,
                    FortuneIncrease = previousMonths.Average(x => x.FortuneIncrease),
                    InvestmentIncrease = previousMonths.Average(x => x.InvestmentIncrease)
                };
            });
        }

        private Dictionary<MonthAndYear, Ledger> CreateMonthlyLedgers(Dictionary<MonthAndYear, IncomeExpensesAndTax> monthlyIncomeExpensesAndTax, MonthlyAccountCategorySums monthlyAccountCategorySums)
        {
            var monthlyLedgers = new Dictionary<MonthAndYear, Ledger>();
            foreach (var pair in monthlyIncomeExpensesAndTax)
            {
                var monthAndYear = pair.Key;
                var incomeExpensesAndTax = pair.Value;

                var income = pair.Value.Income;
                var expenses = pair.Value.Expenses;
                var tax = pair.Value.Tax;
                var variableExpenses = pair.Value.VariableExpenses;
                var fixedExpenses = pair.Value.FixedExpenses;
                var ownPension = monthlyAccountCategorySums.GetOwnPensionSavings(monthAndYear);
                var summedFortunes = monthlyAccountCategorySums.GetFortunesWithoutPension(monthAndYear);
                var values = new Ledger
                {
                    Income = income,
                    IncomeWithoutTaxAndPension = income - tax - ownPension,
                    Expenses = expenses,
                    ExpensesWithoutTax = expenses - tax,
                    VariableExpenses = variableExpenses,
                    FixedExpenses = fixedExpenses,
                    Tax = tax,
                    OwnPension = ownPension,
                    Fortune = summedFortunes,
                    FortuneIncrease = monthlyAccountCategorySums.GetFortune(monthAndYear),
                    InvestmentIncrease = monthlyAccountCategorySums.GetInvestment(monthAndYear),
                };

                monthlyLedgers.Add(monthAndYear, values);
            }

            return monthlyLedgers;
        }
    }
}
