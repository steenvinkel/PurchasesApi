using System.Collections.Generic;
using Business.Models;
using Business.Repositories;

namespace Business.Services
{
    public class AccountService(IAccountRepository accountRepository) : IAccountService
    {
        private readonly IAccountRepository _accountRepository = accountRepository;

        public List<AccountDAO> Get(int userId)
        {
            return _accountRepository.Get(userId);
        }
    }
}
