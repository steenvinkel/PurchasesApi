using Business.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class Posting(int postingId, int subcategoryId, decimal amount, DateTime date)
    {
        public int PostingId { get; set; } = postingId;
        [NotZero]
        public int SubCategoryId { get; set; } = subcategoryId;
        [NotZero]
        public decimal Amount { get; set; } = amount;
        [Required]
        public DateTime Date { get; set; } = date;
    }
}
