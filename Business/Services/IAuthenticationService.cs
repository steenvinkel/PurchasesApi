using System;

namespace Business.Services
{
    public interface IAuthenticationService
    {
        (int, DateTime) GetUserIdAndExpiration(string authToken);
        bool IsValidCredentials(string username, string password);
    }
}
