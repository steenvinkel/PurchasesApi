namespace Legacy.Models
{
    public class LegacyMonthlySumup
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double In { get; set; }
        public double Out { get; set; }
        public double PureOut { get; set; }
        public double Invest { get; set; }
        public double Savings { get; set; }
        public double SavingsLastYear { get; set; }
        public double ExpensesLastYear { get; set; }
        public double MonthsWithoutPay { get; set; }
    }
}