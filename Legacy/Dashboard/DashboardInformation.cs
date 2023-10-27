using System.Collections.Generic;

namespace Legacy.Dashboard
{
    public class DashboardInformation
    {
        public required Dictionary<double, double> FireAgePerReturnRate { get; set; }
        public double MonthsLivableWithoutPay { get; set; }
        public double SavingsRate { get; set; }
        public double Fortune { get; set; }
        public double IncomeWithoutPension { get; set; }
        public double Expenses { get; set; }
        public double NetworthIncreaseFortune { get; set; }
        public double NetworthIncreaseInvestment { get; set; }
        public double VariableExpenses { get; set; }
        public double FixedExpenses { get; set; }
    }
}
