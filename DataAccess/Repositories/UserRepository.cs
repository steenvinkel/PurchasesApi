using Business.Repositories;
using DataAccess.Models;
using System;
using System.Linq;

namespace DataAccess.Repositories
{
    public class UserRepository(PurchasesContext context) : IUserRepository
    {
        private readonly PurchasesContext _context = context;

        public (int, DateTime)? Get(string authToken)
        {
            var user = _context.User.SingleOrDefault(u => u.AuthToken == authToken);

            return user != null
                ? (user.UserId, user.AuthExpire)
                : null;
        }

        public (int UserId, string Name) Get(int userId)
        {
            var user = _context.User.SingleOrDefault(u => u.UserId == userId) ?? throw new ArgumentException($"User with id {userId} does not exist");

            return (user.UserId, user.Username);
        }

        public (string, string)? GetAuthTokenAndPasswordByUsername(string username)
        {
            var user = _context.User.SingleOrDefault(u => u.Username == username);

            return user != null
                ? (user.AuthToken, user.Password)
                : null;
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
