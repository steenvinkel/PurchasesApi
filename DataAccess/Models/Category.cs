using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Category
    {
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
