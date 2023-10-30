using Business.Repositories;
using System;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace Business.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public (int, DateTime) GetUserIdAndExpiration(string authToken)
        {
            var user = _userRepository.Get(authToken) ?? throw new AuthenticationException($"The authentication token ({authToken}) is invalid");

            if (user.AuthExpire < DateTime.Now)
            {
                throw new AuthenticationException("Authentication token has expired");
            }

            return (user.UserId, user.AuthExpire);

        }

        public string IsValidCredentials(string username, string password)
        {
            var (AuthToken, Password) = _userRepository.GetAuthTokenAndPasswordByUsername(username) ?? throw new UnauthorizedAccessException();

            var data = Encoding.UTF8.GetBytes(password);
            var hash = SHA512.HashData(data);

            var hashedInputStringBuilder = new StringBuilder(128);
            foreach (var b in hash)
            {
                hashedInputStringBuilder.Append(b.ToString("X2"));
            }

            var hashedPassword = hashedInputStringBuilder.ToString();

            if (hashedPassword != Password.ToUpper())
            {
                throw new UnauthorizedAccessException();
            }

            return AuthToken;
        }
    }
}
