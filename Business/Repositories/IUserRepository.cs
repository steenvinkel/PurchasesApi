using System;

namespace Business.Repositories
{
    public interface IUserRepository
    {
        (int, DateTime)? Get(string authToken);
        (int UserId, string Name) Get(int userId);
        (string AuthToken, string Password)? GetAuthTokenAndPasswordByUsername(string username);
        void SaveAuthToken(int userId, string authToken, DateTime authExpire);
    }
}
