using System.Collections.Generic;

namespace Legacy.Dashboard
{
    public class DashboardInformation
    {
        public Dictionary<double, double> FireAgePerReturnRate { get; set; }
        public double MonthsLivableWithoutPay { get; set; }
        public double SavingsRate { get; set; }
        public double Fortune { get; set; }
        public double IncomeWithoutTaxAndPension { get; set; }
        public double ExpensesWithoutTax { get; set; }
    }
}
