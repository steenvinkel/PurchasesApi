using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IAccountRepository
    {
        List<Account> GetAccounts(int userId);
    }
}
