using Business.Models;
using System;
using System.Collections.Generic;

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

        public static LegacyPosting Map(Posting posting, Dictionary<int, string> subcategoryIdMap)
        {
            var description = subcategoryIdMap[posting.SubcategoryId];

            return new LegacyPosting
            {
                Posting_id = posting.PostingId,
                Amount = posting.Amount,
                Date = posting.Date,
                Description = description,
                Latitude = decimal.Zero,
                Longitude = decimal.Zero,
                Accuracy = decimal.Zero
            };
        }

        public static Posting Map(LegacyPosting posting, int userId, Dictionary<string, int> subcategoryIdMap)
        {
            var subcategoryId = subcategoryIdMap[posting.Description];

            return new Posting(posting.Posting_id, userId, subcategoryId, posting.Amount, posting.Date);
        }
    }
}
