namespace Business.Models
{
    public class SubCategory
    {
        public SubCategory(int id, int categoryId, string name, string? color)
        {
            Id = id;
            CategoryId = categoryId;
            Name = name;
            Color = color;
        }

        public int Id { get; }
        public int CategoryId { get; }
        public string Name { get; }
        public string? Color { get; }
    }
}
