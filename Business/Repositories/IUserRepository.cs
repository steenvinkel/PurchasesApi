using Business.Models;
using System;

namespace Business.Repositories
{
    public interface IUserRepository
    {
        User? Get(string authToken);
        (string AuthToken, string Password)? GetAuthTokenAndPasswordByUsername(string username);
        void SaveAuthToken(int userId, string authToken, DateTime authExpire);
    }
}
