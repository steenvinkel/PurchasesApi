using System;

namespace Business.Models
{
    public class Posting(int postingId, int userId, int subcategoryId, decimal amount, DateTime date)
    {
        public int PostingId { get; set; } = postingId;
        public int UserId { get; set; } = userId;
        public int SubcategoryId { get; set; } = subcategoryId;
        public decimal Amount { get; set; } = amount;
        public DateTime Date { get; set; } = date;
    }
}
