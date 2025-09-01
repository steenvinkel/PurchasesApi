using System.Collections.Generic;

namespace Business.Models
{
    public class IncomeCategory(int categoryId, string name, string color, List<SubCategoryBase> subcategories)
        : CategoryBase(categoryId, name, color, subcategories)
    {
    }
}
