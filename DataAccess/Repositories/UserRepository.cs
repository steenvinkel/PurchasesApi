using Business.Repositories;
using DataAccess.Models;
using System.Linq;

namespace DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PurchasesContext _context;

        public UserRepository(PurchasesContext context)
        {
            _context = context;
        }

        public Business.Models.User Get(string authToken)
        {
            var user = _context.User.SingleOrDefault(u => u.AuthToken == authToken);

            return user != null ? new Business.Models.User(user.UserId, user.Username, user.AuthToken, user.AuthExpire) : null;
        }
    }
}
