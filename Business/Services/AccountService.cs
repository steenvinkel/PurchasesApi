using System.Collections.Generic;
using Business.Models;
using Business.Repositories;

namespace Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public List<Account> Get(int userId)
        {
            return _accountRepository.Get(userId);
        }
    }
}
