namespace Legacy.Models
{
    public class LegacyMonthlySumup
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public double In { get; set; }
        public double Out { get; set; }
        public double pureOut { get; set; }
        public double invest { get; set; }
        public double savings { get; set; }
        public double savingsWithoutOwnContribution { get; set; }
        public double savingsLastYear { get; set; }
        public double expensesLastYear { get; set; }
        public double monthsWithoutPay { get; set; }
        public double extra { get; set; }
    }
}