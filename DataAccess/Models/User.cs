using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public short Admin { get; set; }
        public DateTime JoinDate { get; set; }
        public short Active { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
        public required string AuthToken { get; set; }
        public DateTime AuthExpire { get; set; }
        public string? ObjectId { get; set; }
    }
}
