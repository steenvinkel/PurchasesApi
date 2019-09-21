namespace Business.Models
{
    public class Ledger
    {
        public double Income { get; set; } = 0;
        public double IncomeWithoutTaxAndPension { get; set; } = 0;
        public double Expenses { get; set; } = 0;
        public double ExpensesWithoutTax { get; set; } = 0;
        public double OwnPension { get; set; } = 0;
        public double Tax { get; set; } = 0;
        public double Fortune { get; set; } = 0;
    }
}
