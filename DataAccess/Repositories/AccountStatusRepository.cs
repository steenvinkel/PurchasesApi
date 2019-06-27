using Business.Models;
using Business.Repositories;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Repositories
{
    public class AccountStatusRepository : IAccountStatusRepository
    {
        public readonly PurchasesContext _context;

        public AccountStatusRepository(PurchasesContext context)
        {
            _context = context;
        }

        public List<Business.Models.AccountStatus> Get(int userId)
        {
            var accountStatuses = _context.AccountStatus.Where(accountStatus => accountStatus.UserId == userId).ToList();

            return accountStatuses.Select(Map).ToList();
        }

        private Business.Models.AccountStatus Map(Models.AccountStatus a)
        {
            return new Business.Models.AccountStatus(a.AccountStatusId, a.AccountId, a.Date, a.Amount);
        }
    }
}
