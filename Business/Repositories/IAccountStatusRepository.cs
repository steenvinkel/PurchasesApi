using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IAccountStatusRepository
    {
        List<AccountStatus> Get(int userId);
        List<AccountStatus> Add(int userId, List<AccountStatus> accountStatuses);
        List<AccountStatus> Update(int userId, List<AccountStatus> updatedAccountStatuses);
    }
}
