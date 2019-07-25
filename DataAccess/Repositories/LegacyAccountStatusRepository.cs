using DataAccess.Models;
using Legacy.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacyAccountStatusRepository : ILegacyAccountStatusRepository
    {
        private readonly PurchasesContext _context;

        public LegacyAccountStatusRepository(PurchasesContext context)
        {
            _context = context;
        }

        public List<LegacyAccountStatus> Post(List<LegacyAccountStatus> accountStatuses, int userId)
        {
            List<AccountStatus> newAccountStatuses = new List<AccountStatus>();
            foreach(var accountStatus in accountStatuses)
            {
                var newAccountStatus = new AccountStatus
                {
                    AccountId = accountStatus.Account_id,
                    Date = new DateTime(accountStatus.Year, accountStatus.Month, 1),
                    Amount = accountStatus.Amount,
                    CreatedOn = DateTime.Now,
                    UserId = userId,
                };

                newAccountStatuses.Add(newAccountStatus);
                _context.AccountStatus.Add(newAccountStatus);

            }
            _context.SaveChanges();

            return newAccountStatuses.Select(Map).ToList();
        }
        public List<LegacyAccountStatus> Put(List<LegacyAccountStatus> accountStatuses, int userId)
        {
            var ids = accountStatuses.Select(x => x.Account_status_id).ToList();
            List<AccountStatus> existingAccountStatuses = _context.AccountStatus
                .Where(x => ids.Contains(x.AccountStatusId))
                .Where(x=> x.UserId == userId)
                .ToList();

            if (existingAccountStatuses.Count != accountStatuses.Count)
            {
                throw new Exception("Could not find all the specified AccountStatusIds");
            }

            var accountStatusMap = accountStatuses.ToDictionary(x => x.Account_status_id);
            foreach(var existingAccountStatus in existingAccountStatuses)
            {
                var accountStatus = accountStatusMap[existingAccountStatus.AccountStatusId];

                existingAccountStatus.AccountId = accountStatus.Account_id;
                existingAccountStatus.Date = new DateTime(accountStatus.Year, accountStatus.Month, 1);
                existingAccountStatus.Amount = accountStatus.Amount;
            }
            _context.SaveChanges();

            return existingAccountStatuses.Select(Map).ToList();
        }

        private LegacyAccountStatus Map(AccountStatus accountStatus)
        {
            return new LegacyAccountStatus(accountStatus.AccountId, accountStatus.AccountStatusId, accountStatus.Amount, accountStatus.Date.Year, accountStatus.Date.Month);
        }
    }
}
