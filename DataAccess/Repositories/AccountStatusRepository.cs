using Business.Repositories;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class AccountStatusRepository : IAccountStatusRepository
    {
        public readonly PurchasesContext _context;

        public AccountStatusRepository(PurchasesContext context)
        {
            _context = context;
        }

        public List<Business.Models.AccountStatusDAO> Get(int userId)
        {
            var accountStatuses = _context.AccountStatus.Where(accountStatus => accountStatus.UserId == userId).ToList();

            return accountStatuses.Select(Map).ToList();
        }

        public List<Business.Models.AccountStatusDAO> Add(int userId, List<Business.Models.AccountStatusDAO> accountStatuses)
        {
            var newAccountStatuses = new List<AccountStatus>();
            foreach(var accountStatus in accountStatuses)
            {
                var newAccountStatus = CreateNewAccountStatus(accountStatus, userId);

                newAccountStatuses.Add(newAccountStatus);
                _context.AccountStatus.Add(newAccountStatus);
            }
            _context.SaveChanges();

            return newAccountStatuses.Select(Map).ToList();
        }

        public List<Business.Models.AccountStatusDAO> Update(int userId, List<Business.Models.AccountStatusDAO> updatedAccountStatuses)
        {
            var ids = updatedAccountStatuses.Select(a => a.AccountStatusId).ToList();
            List<AccountStatus> existingAccountStatuses = _context.AccountStatus
                .Where(x => ids.Contains(x.AccountStatusId))
                .Where(x=> x.UserId == userId)
                .ToList();

            if (existingAccountStatuses.Count != updatedAccountStatuses.Count)
            {
                throw new Exception("Could not find all the specified AccountStatusIds");
            }

            var updatedAccountStatusMap = updatedAccountStatuses.ToDictionary(x => x.AccountStatusId);
            foreach(var existingAccountStatus in existingAccountStatuses)
            {
                var updateAccountStatus = updatedAccountStatusMap[existingAccountStatus.AccountStatusId];

                existingAccountStatus.AccountId = updateAccountStatus.AccountId;
                existingAccountStatus.Date = updateAccountStatus.Date;
                existingAccountStatus.Amount = updateAccountStatus.Amount;
            }
            _context.SaveChanges();

            return existingAccountStatuses.Select(Map).ToList();
        }

        private AccountStatus CreateNewAccountStatus(Business.Models.AccountStatusDAO accountStatus, int userId)
        {
            return new AccountStatus
            {
                AccountId = accountStatus.AccountId,
                Date = accountStatus.Date,
                Amount = accountStatus.Amount,
                CreatedOn = DateTime.Now,
                UserId = userId
            };
        }

        private Business.Models.AccountStatusDAO Map(AccountStatus a)
        {
            return new Business.Models.AccountStatusDAO(a.AccountStatusId, a.AccountId, a.Date, a.Amount);
        }
    }
}
