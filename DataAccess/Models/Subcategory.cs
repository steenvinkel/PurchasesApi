using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Subcategory
    {
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
