using Business.Repositories;
using System;
using System.Security.Authentication;

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
            var user = _userRepository.Get(authToken);
            if (user == null)
            {
                throw new AuthenticationException($"The authentication token ({authToken}) is invalid");
            }

            if (user.AuthExpire < DateTime.Now)
            {
                throw new AuthenticationException("Authentication token has expired");
            }

            return (user.UserId, user.AuthExpire);

        }
    }
}
