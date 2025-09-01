using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Business.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(IncomeCategory), "Income")]
    [JsonDerivedType(typeof(ExpenseCategory), "Expense")]
    public abstract class CategoryBase(int categoryId, string name, string color, List<SubCategoryBase> subcategories)
    {
        public int CategoryId { get; } = categoryId;
        public string Name { get; } = name;
        public string Color { get; } = color;
        public List<SubCategoryBase> Subcategories { get; } = subcategories;
    }
}
