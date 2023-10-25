using Business.Models;

namespace Legacy.Models
{
    public class MonthlyTypeSum
    {
        public required MonthAndYear MonthAndYear { get; set; }
        public required string Type { get; set; }
        public double Sum { get; set; }
    }
}
