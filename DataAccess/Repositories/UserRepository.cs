using Business.Models;
using Business.Repositories;
using DataAccess.Models;
using System;
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

            return Map(user);
        }

        public (string, DateTime, string, int) GetByUsername(string username)
        {
            var user = _context.User.SingleOrDefault(u => u.Username == username);

            return (user.AuthToken, user.AuthExpire, user.Password, user.UserId);
        }

        private static Business.Models.User Map(Models.User user)
        {
            return user != null ? new Business.Models.User(user.UserId, user.Username, user.AuthToken, user.AuthExpire) : null;
        }

        public void SaveAuthToken(int userId, string authToken, DateTime authExpire)
        {
            var user = _context.User.Single(u => u.UserId == userId);
            user.AuthToken = authToken;
            user.AuthExpire = authExpire;
            _context.SaveChanges();
        }
    }
}
