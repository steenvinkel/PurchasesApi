using System.Text.Json.Serialization;

namespace Business.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ExpenseSubCategoryTypes
    {
        Variable = 0,
        Fixed = 1,
    }

    public static class ExpenseSubCategoryTypesConverter
    {
        public static ExpenseSubCategoryTypes ToEnum(string SubCategoryType)
        {
            return SubCategoryType switch
            {
                "variable" => ExpenseSubCategoryTypes.Variable,
                "fixed" => ExpenseSubCategoryTypes.Fixed,
                _ => throw new System.ArgumentOutOfRangeException($"SubCategoryType '{SubCategoryType}' is not valid"),
            };
        }
    }
}
