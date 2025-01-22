using System.Collections.Generic;

namespace Legacy.Dashboard
{
    public class DashboardInformation
    {
        public required Dictionary<decimal, decimal> FireAgePerReturnRate { get; set; }
        public decimal? MonthsLivableWithoutPay { get; set; }
        public decimal? SavingsRate { get; set; }
        public decimal Fortune { get; set; }
        public decimal InvestedFortune { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetworthIncreaseFortune { get; set; }
        public decimal NetworthIncreaseInvestment { get; set; }
        public decimal VariableExpenses { get; set; }
        public decimal FixedExpenses { get; set; }
    }
}
