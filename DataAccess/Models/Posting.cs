using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Posting
    {
        public int PostingId { get; set; }
        public int UserId { get; set; }
        public int? SubcategoryId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Accuracy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
