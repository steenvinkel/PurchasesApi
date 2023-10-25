using System;

namespace DataAccess.Models
{
    public partial class Category
    {
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public required string Name { get; set; }
        public string? Color { get; set; }
        public required string Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
