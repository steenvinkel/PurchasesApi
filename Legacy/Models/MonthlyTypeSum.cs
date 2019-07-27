using Business.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Models
{
    public class MonthlyTypeSum
    {
        public MonthAndYear MonthAndYear { get; set; }
        public string Type { get; set; }
        public double Sum { get; set; }
    }
}
