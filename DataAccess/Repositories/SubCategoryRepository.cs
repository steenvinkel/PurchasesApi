using Business.Models;
using Business.Repositories;
using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        public readonly PurchasesContext _context;

        public SubCategoryRepository(PurchasesContext context)
        {
            _context = context;
        }

        public List<SubCategory> GetList(int userId)
        {
            var subcategories = (from subcategory in _context.Subcategory
                                   join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                                   where category.UserId == userId
                                   select subcategory).ToList();

            return subcategories.Select(Map).ToList();
        }

        private SubCategory Map(Subcategory subcategory)
        {
            return new SubCategory(subcategory.SubcategoryId, subcategory.CategoryId, subcategory.Name, subcategory.Color);
        }
    }
}
