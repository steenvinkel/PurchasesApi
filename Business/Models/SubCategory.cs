namespace Business.Models
{
    public class SubCategory(int id, int categoryId, string name, string? color)
    {
        public int SubCategoryId { get; } = id;
        public int CategoryId { get; } = categoryId;
        public string Name { get; } = name;
        public string? Color { get; } = color;
    }
}
