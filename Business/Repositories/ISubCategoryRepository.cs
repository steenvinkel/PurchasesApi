using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface ISubCategoryRepository
    {
        List<CategoryBase> GetCategories(int userId);
        List<SubCategory> GetList(int userId);
    }
}
