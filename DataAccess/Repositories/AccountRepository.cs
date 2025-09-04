using Business.Repositories;
using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public readonly PurchasesContext _context;

        public AccountRepository(PurchasesContext context)
        {
            _context = context;
        }

        public List<Business.Models.Account> GetAccounts(int userId)
        {
            var accountsDao = (from type in _context.AccumulatedCategory
                               join account in _context.Account on type.AccumulatedCategoryId equals account.AccumulatedCategoryId
                               where account.UserId == userId
                               select new
                               {
                                   Type = type.Name,
                                   account.AccountId,
                                   account.Name
                               });

            var accountToStatusesMap = accountsDao.ToDictionary(account => account.AccountId, account => new List<Business.Models.AccountStatus>());

            var accountStatusesDao = _context.AccountStatus.Where(accountStatus => accountStatus.UserId == userId).ToList();

            foreach(var asDao in accountStatusesDao)
            {
                var accountStatus = new Business.Models.AccountStatus(asDao.AccountStatusId, asDao.Date, asDao.Amount);

                accountToStatusesMap[asDao.AccountId].Add(accountStatus);
            }

            var newAccounts = accountsDao.Select(a => new Business.Models.Account(a.AccountId, a.Name, a.Type, accountToStatusesMap[a.AccountId])).ToList();

            return newAccounts;
        }
    }
}
