using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface IAccountStatusRepository
    {
        void Add(int userId, int accountId, AccountStatus accountStatuses);
        void Update(int userId, int accountId, AccountStatus accountStatus);
    }
}
