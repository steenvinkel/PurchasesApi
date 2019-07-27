namespace Business
{
    public static class Calculate
    {
        public static double SavingsRate(double income, double expenses)
        {
            return income <= 0
                ? 0
                : (income - expenses) / income * 100;
        }
    }
}
