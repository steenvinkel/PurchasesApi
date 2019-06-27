namespace Business.Models
{
    public class Account
    {
        public Account(int accountId, string name, int accumulatedCategoryId)
        {
            AccountId = accountId;
            Name = name;
            AccumulatedCategoryId = accumulatedCategoryId;
        }

        public int AccountId { get; private set; }
        public string Name { get; private set; }
        public int AccumulatedCategoryId { get; private set; }
    }
}
