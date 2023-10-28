namespace Business.Models
{
    public class Ledger
    {
        public double Income { get; set; } = 0;
        public double Expenses { get; set; } = 0;
        public double VariableExpenses { get; set; } = 0;
        public double FixedExpenses { get; set; } = 0;
        public double Fortune { get; set; } = 0;
        public double FortuneIncrease { get; set; } = 0;
        public double InvestmentIncrease { get; set; } = 0;
    }
}
