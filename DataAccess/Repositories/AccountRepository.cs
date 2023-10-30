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

        public List<Business.Models.Account> Get(int userId)
        {
            var accounts = _context.Account.Where(account => account.UserId == userId).ToList();

            return accounts.Select(Map).ToList();
        }

        private Business.Models.Account Map(Models.Account account)
        {
            return new Business.Models.Account(account.AccountId, account.Name, account.AccumulatedCategoryId);
        }
    }
}
