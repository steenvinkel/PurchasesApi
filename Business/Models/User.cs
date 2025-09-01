using System.Collections.Generic;

namespace Business.Models
{
    public class User(int userId, string username, List<CategoryBase> categories, List<Account> accounts)
    {
        public int UserId { get; } = userId;
        public string Username { get; } = username;
        public List<CategoryBase> Categories { get; } = categories;
        public List<Account> Accounts { get; } = accounts;
    }
}
