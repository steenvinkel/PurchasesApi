using Business.Models;

namespace Legacy.Models
{
    public class LegacyMonthlySubcategorySum
    {
        public MonthAndYear MonthAndYear { get; set; }
        public int SubcategoryId { get; set; }
        public double Sum { get; set; }
    }
}
