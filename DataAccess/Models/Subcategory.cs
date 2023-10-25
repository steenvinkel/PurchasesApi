using System;

namespace DataAccess.Models
{
    public partial class Subcategory
    {
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
        public required string Name { get; set; }
        public string? Color { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
