using System.Linq;

namespace Business.Services
{
    public static class Calculator
    {
        public static double CalculateMonthsLivableWithoutPay(double fortune, double monthlyExpenses)
        {
            if (monthlyExpenses <= 0)
            {
                return double.PositiveInfinity;
            }
            if (fortune <= 0)
            {
                return 0;
            }

            return fortune/monthlyExpenses;
        }

        public static double SavingsRate(double income, double expenses)
        {
            if (income < 0 || expenses < 0)
            {
                return 0;
            }

            return income == 0
                ? double.NegativeInfinity
                : (income - expenses) / income * 100;
        }

        public static double FireAge(double income, double expenses, double fortune, double returnRate, int currentAge, int pensionAge)
        {
            var yearsWorking = YearsToWork(income, expenses, fortune, returnRate, pensionAge - currentAge);

            return yearsWorking == int.MaxValue
                ? int.MaxValue
                : currentAge + yearsWorking;
        }

        private static int YearsToWork(double income, double expenses, double fortune, double returnRate, int yearsToPension)
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

        public static double CalculateLifeScenario(double income, double expenses, double startAmount, double returnRate, int yearsWorking, int yearsNotWorking)
        {
            double amountAfterWorking = SavingsAfterYears(income, expenses, startAmount, returnRate, yearsWorking);
            double amountAfterNotWorking = SavingsAfterYears(0, expenses, amountAfterWorking, returnRate, yearsNotWorking);
            return amountAfterNotWorking;
        }

        private static double SavingsAfterYears(double income, double expenses, double startAmount, double returnRate, int years)
        {
            return Enumerable
                .Range(0, years)
                .Aggregate(startAmount, (amount, _) => SavingsAfterOneYear(income - expenses, amount, returnRate));
        }

        public static double SavingsAfterOneYear(double monthlyChange, double start, double returnRate)
        {
            return start * (1 + returnRate) + monthlyChange * 12;
        }
    }
}
