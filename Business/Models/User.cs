using System;

namespace Business.Models
{
    public class User(int userId, string username, string authToken, DateTime authExpire)
    {
        public int UserId { get; } = userId;
        public string Username { get; } = username;
        public string AuthToken { get; } = authToken;
        public DateTime AuthExpire { get; } = authExpire;
    }
}
