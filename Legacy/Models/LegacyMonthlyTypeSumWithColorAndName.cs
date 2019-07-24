﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Models
{
    public class LegacyMonthlyTypeSumWithColorAndName
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Type { get; set; }
        public double Sum { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
