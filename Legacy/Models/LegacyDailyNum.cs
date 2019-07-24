using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Models
{
    public class LegacyDailyNum
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public double NumPurchases { get; set; }
    }
}
