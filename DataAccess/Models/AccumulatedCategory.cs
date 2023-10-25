namespace DataAccess.Models
{
    public partial class AccumulatedCategory
    {
        public int AccumulatedCategoryId { get; set; }
        public required string Name { get; set; }
        public required string Color { get; set; }
        public int UserId { get; set; }
    }
}
