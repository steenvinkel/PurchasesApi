namespace Business.Models
{
    public class AccountDAO(int accountId, string name, int accumulatedCategoryId)
    {
        public int AccountId { get; private set; } = accountId;
        public string Name { get; private set; } = name;
        public int AccumulatedCategoryId { get; private set; } = accumulatedCategoryId;
    }
}
