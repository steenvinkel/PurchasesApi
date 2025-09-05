using Business.Repositories;
using DataAccess.Models;
using System;
using System.Linq;

namespace DataAccess.Repositories
{
    public class AccountStatusRepository(PurchasesContext context) : IAccountStatusRepository
    {
        public readonly PurchasesContext _context = context;

        public void Add(int userId, int accountId, Business.Models.AccountStatus accountStatuses)
        {
            var newAccountStatus = new AccountStatus
            {
                AccountId = accountId,
                Date = accountStatuses.Date.Date,
                Amount = accountStatuses.Amount,
                CreatedOn = DateTime.Now,
                UserId = userId
            };
            _context.AccountStatus.Add(newAccountStatus);
            _context.SaveChanges();
        }

        public void Update(int userId, int accountId, Business.Models.AccountStatus accountStatus)
        {
            var existingAccountStatus = _context.AccountStatus
                .FirstOrDefault(x => x.AccountStatusId == accountStatus.AccountStatusId);

            if (existingAccountStatus == null)
            {
                throw new Exception("AccountStatus not found for update.");
            }

            existingAccountStatus.Date = accountStatus.Date.Date;
            existingAccountStatus.Amount = accountStatus.Amount;

            _context.SaveChanges();
        }
    }
}
