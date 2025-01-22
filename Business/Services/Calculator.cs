using System.Linq;

namespace Business.Services
{
    public static class Calculator
    {
        public static decimal? CalculateMonthsLivableWithoutPay(decimal fortune, decimal monthlyExpenses)
        {
            if (monthlyExpenses <= 0)
            {
                return null;
            }
            if (fortune <= 0)
            {
                return 0;
            }

            return fortune/monthlyExpenses;
        }

        public static decimal? SavingsRate(decimal income, decimal expenses)
        {
            if (income < 0 || expenses < 0)
            {
                return 0;
            }

            return income == 0
                ? null
                : (income - expenses) / income * 100;
        }

        public static decimal FireAge(decimal income, decimal expenses, decimal fortune, decimal returnRate, int currentAge, int pensionAge)
        {
            var yearsWorking = YearsToWork(income, expenses, fortune, returnRate, pensionAge - currentAge);

            return yearsWorking == int.MaxValue
                ? int.MaxValue
                : currentAge + yearsWorking;
        }

        private static int YearsToWork(decimal income, decimal expenses, decimal fortune, decimal returnRate, int yearsToPension)
        {
            for(var yearsWorking = 1; yearsWorking <= yearsToPension; yearsWorking++)
            {
                var amount = CalculateLifeScenario(income, expenses, fortune, returnRate, yearsWorking, yearsToPension - yearsWorking);

                if (amount >= 0)
                {
                    return yearsWorking;
                }
            }

            return int.MaxValue;
        }

        public static decimal CalculateLifeScenario(decimal income, decimal expenses, decimal startAmount, decimal returnRate, int yearsWorking, int yearsNotWorking)
        {
            decimal amountAfterWorking = SavingsAfterYears(income, expenses, startAmount, returnRate, yearsWorking);
            decimal amountAfterNotWorking = SavingsAfterYears(0, expenses, amountAfterWorking, returnRate, yearsNotWorking);
            return amountAfterNotWorking;
        }

        private static decimal SavingsAfterYears(decimal income, decimal expenses, decimal startAmount, decimal returnRate, int years)
        {
            return Enumerable
                .Range(0, years)
                .Aggregate(startAmount, (amount, _) => SavingsAfterOneYear(income - expenses, amount, returnRate));
        }

        public static decimal SavingsAfterOneYear(decimal monthlyChange, decimal start, decimal returnRate)
        {
            return start * (1 + returnRate) + monthlyChange * 12;
        }
    }
}
