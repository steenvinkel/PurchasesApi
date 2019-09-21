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

        public Dictionary<int, DashboardInformation> GetDashboards(int userId, List<int> monthsInDashboard, List<double> returnRates, int currentAge, int pensionAge)
        {
            var monthlyIncomeExpensesAndTax = _postingQueryRepository.GetMonthlyIncomeExpenseAndTax(userId);
            var monthlyAccountCategorySums = _monthlyAccountStatusRepository.GetAccumulatedCategorySums(userId);

            var monthlyLedgers = CreateMonthlyLedgers(monthlyIncomeExpensesAndTax, monthlyAccountCategorySums);

            var dashboards = monthsInDashboard.ToDictionary(numMonths => numMonths, numMonths => 
                    CalculateDashboard(returnRates, currentAge, pensionAge, numMonths, monthlyLedgers));

            return dashboards;
        }

        private DashboardInformation CalculateDashboard(List<double> returnRates, int currentAge, int pensionAge, int numMonths, Dictionary<MonthAndYear, Ledger> monthlyLedgers)
        {
            var ledger = CalculateAverageLedger(monthlyLedgers, numMonths);
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
            };

            return dashboardInformation;
        }

        private Ledger CalculateAverageLedger(Dictionary<MonthAndYear, Ledger> monthlyLedgers, int numMonths)
        {
            return CalculateAverageLedgers(monthlyLedgers, numMonths)[MonthAndYear.Now];
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
                    Tax = previousMonths.Average(x => x.Tax),
                    OwnPension = previousMonths.Average(x => x.OwnPension),
                    Fortune = previousMonthlyLedger?.Fortune ?? 0,
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
                var ownPension = monthlyAccountCategorySums.GetOwnPensionSavings(monthAndYear);
                var summedFortunes = monthlyAccountCategorySums.GetFortunesWithoutPension(monthAndYear);
                var values = new Ledger
                {
                    Income = income,
                    IncomeWithoutTaxAndPension = income - tax - ownPension,
                    Expenses = expenses,
                    ExpensesWithoutTax = expenses - tax,
                    Tax = tax,
                    OwnPension = ownPension,
                    Fortune = summedFortunes
                };

                monthlyLedgers.Add(monthAndYear, values);
            }

            return monthlyLedgers;
        }
    }
}
