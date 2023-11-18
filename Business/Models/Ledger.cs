namespace Business.Models
{
    public class Ledger
    {
        public decimal Income { get; set; } = 0;
        public decimal Expenses { get; set; } = 0;
        public decimal VariableExpenses { get; set; } = 0;
        public decimal FixedExpenses { get; set; } = 0;
        public decimal Fortune { get; set; } = 0;
        public decimal InvestedFortune { get; set; } = 0;
        public decimal FortuneIncrease { get; set; } = 0;
        public decimal InvestmentIncrease { get; set; } = 0;
    }
}
