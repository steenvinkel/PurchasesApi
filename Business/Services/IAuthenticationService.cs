using System;

namespace Business.Services
{
    public interface IAuthenticationService
    {
        (int, DateTime) GetUserIdAndExpiration(string authToken);
        string IsValidCredentials(string username, string password);
    }
}
