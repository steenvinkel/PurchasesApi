using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IAccountStatusRepository
    {
        List<AccountStatus> Get(int userId);
    }
}
