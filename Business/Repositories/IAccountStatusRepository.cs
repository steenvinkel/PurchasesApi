using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IAccountStatusRepository
    {
        List<AccountStatusDAO> Get(int userId);
        List<AccountStatusDAO> Add(int userId, List<AccountStatusDAO> accountStatuses);
        List<AccountStatusDAO> Update(int userId, List<AccountStatusDAO> updatedAccountStatuses);
    }
}
