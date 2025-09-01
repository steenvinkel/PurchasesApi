using System.Text.Json.Serialization;

namespace Business.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(IncomeSubCategory), "Income")]
    [JsonDerivedType(typeof(VariableExpenseSubCategory), "VariableExpense")]
    [JsonDerivedType(typeof(FixedExpenseSubCategory), "FixedExpense")]
    public abstract class SubCategoryBase(int subCategoryId, string name, string? color)
    {
        public int SubCategoryId { get; } = subCategoryId;
        public string Name { get; } = name;
        public string? Color { get; } = color;
    }
}
