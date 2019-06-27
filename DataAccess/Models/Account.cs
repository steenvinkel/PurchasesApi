using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public partial class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int AccumulatedCategoryId { get; set; }
    }
}
