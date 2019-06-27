using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class AccumulatedCategory
    {
        public int AccumulatedCategoryId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int UserId { get; set; }
    }
}
