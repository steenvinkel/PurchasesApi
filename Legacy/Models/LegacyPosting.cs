using System;

namespace Legacy.Models
{
    public class LegacyPosting
    {
        public int Posting_id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public required string Description { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Accuracy { get; set; }
    }
}
