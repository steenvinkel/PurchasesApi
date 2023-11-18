using System;

namespace DataAccess.Models
{
    public partial class Posting
    {
        public int PostingId { get; set; }
        public int UserId { get; set; }
        public int? SubcategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Accuracy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
