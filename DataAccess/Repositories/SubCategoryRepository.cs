using Business.Models;
using Business.Repositories;
using DataAccess.Constants;
using DataAccess.Models;
using System;
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

        public List<Business.Models.SubCategory> GetList(int userId)
        {
            var subcategories = (from subcategory in _context.SubCategory
                                   join category in _context.CategoryForUser(userId) on subcategory.CategoryId equals category.CategoryId
                                   select subcategory).ToList();

            return subcategories.Select(Map).ToList();
        }

        public List<Business.Models.CategoryBase> GetCategories(int userId)
        {
            var categories = (from category in _context.CategoryForUser(userId)
                              select category).ToList();

            var categoriesMap = categories.ToDictionary(cat => cat.CategoryId);
            var categoriesToIncomeSubCategoriesMap = categories.Where(cat => cat.Type == CategoryProperties.Type.In).ToDictionary(cat => cat.CategoryId, cat => new List<SubCategoryBase>());
            var categoriesToExpenseSubCategoriesMap = categories.Where(cat => cat.Type == CategoryProperties.Type.Out).ToDictionary(cat => cat.CategoryId, cat => new List<SubCategoryBase>());

            var subcategories = (from subcategory in _context.SubCategory
                                 join category in _context.CategoryForUser(userId) on subcategory.CategoryId equals category.CategoryId
                                 select subcategory).ToList();

            foreach (var subcategory in subcategories)
            {
                var category = categoriesMap[subcategory.CategoryId];

                if (category.Type == CategoryProperties.Type.In)
                {
                    var newSubcategory = new IncomeSubCategory(subcategory.SubcategoryId, subcategory.Name, subcategory.Color);
                    categoriesToIncomeSubCategoriesMap[category.CategoryId].Add(newSubcategory);
                }
                else
                {
                    SubCategoryBase newSubCategory;
                    if (subcategory.Type == SubCategoryProperties.Type.Variable)
                    {
                        newSubCategory = new VariableExpenseSubCategory(subcategory.SubcategoryId, subcategory.Name, subcategory.Color);
                    } else if (subcategory.Type == SubCategoryProperties.Type.Fixed)
                    {
                        newSubCategory = new FixedExpenseSubCategory(subcategory.SubcategoryId, subcategory.Name, subcategory.Color);
                    } else
                    {
                        throw new ArgumentException($"Unknown Subcategory.Type value: {subcategory.Type}");
                    }
                    categoriesToExpenseSubCategoriesMap[category.CategoryId].Add(newSubCategory);
                }
            }

            var mappedCategories = categories.Select(cat => Map(cat, categoriesToIncomeSubCategoriesMap, categoriesToExpenseSubCategoriesMap)).ToList();

            return mappedCategories;
        }

        private static Business.Models.CategoryBase Map(Models.Category cat, Dictionary<int, List<SubCategoryBase>> categoriesToIncomeSubCategoriesMap, Dictionary<int, List<SubCategoryBase>> categoriesToExpenseSubCategoriesMap)
        {
            return cat.Type == CategoryProperties.Type.In
                ? new Business.Models.IncomeCategory(cat.CategoryId, cat.Name, cat.Color ?? string.Empty, categoriesToIncomeSubCategoriesMap[cat.CategoryId])
                : new Business.Models.ExpenseCategory(cat.CategoryId, cat.Name, cat.Color ?? string.Empty, categoriesToExpenseSubCategoriesMap[cat.CategoryId]);
        }

        private Business.Models.SubCategory Map(Models.SubCategory subcategory)
        {
            return new Business.Models.SubCategory(subcategory.SubcategoryId, subcategory.CategoryId, subcategory.Name, subcategory.Color);
        }
    }
}
