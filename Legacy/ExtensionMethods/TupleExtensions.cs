using Business;

namespace Legacy.ExtensionMethods
{
    public static class TupleExtensions
    {
        public static double CalculateSavingsRate(this (double Income, double Expenses) tuple)
        {
            return Calculate.SavingsRate(tuple.Income, tuple.Expenses);
        }
    }
}
