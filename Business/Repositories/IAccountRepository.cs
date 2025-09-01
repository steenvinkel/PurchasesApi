using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IAccountRepository
    {
        List<AccountDAO> Get(int userId);
        List<Account> GetAccounts(int userId);
    }
}
