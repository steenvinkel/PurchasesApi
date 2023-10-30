using Business.Models;
using System.Collections.Generic;

namespace Business.Repositories
{
    public interface ISubCategoryRepository
    {
        List<SubCategory> GetList(int userId);
    }
}
