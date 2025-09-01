using Business.Models;
using Business.Repositories;

namespace Business.Services
{
    public class UserService(IUserRepository userRepository, ISubCategoryRepository subCategoryRepository, IAccountRepository accountRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ISubCategoryRepository _subCategoryRepository = subCategoryRepository;
        private readonly IAccountRepository _accountRepository = accountRepository;

        public User Get(int userId)
        {
            var (UserId, Name) = _userRepository.Get(userId);

            var categories = _subCategoryRepository.GetCategories(userId);

            var accounts = _accountRepository.GetAccounts(userId);

            return new User(UserId, Name, categories, accounts);
        }
    }
}
