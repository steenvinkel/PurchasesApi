using System;

namespace DataAccess.Models
{
    public partial class AccountStatus
    {
        public int AccountStatusId { get; set; }
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
