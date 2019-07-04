using System;

namespace Business.Models
{
    public class User
    {
        public User(int userId, string username, string authToken, DateTime authExpire)
        {
            UserId = userId;
            Username = username;
            AuthToken = authToken;
            AuthExpire = authExpire;
        }

        public int UserId { get; }
        public string Username { get; }
        public string AuthToken { get; }
        public DateTime AuthExpire { get; }
    }
}
