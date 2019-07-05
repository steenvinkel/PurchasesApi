using DataAccess.Models;
using Legacy.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacySummaryRepository : ILegacySummaryRepository
    {
        private readonly PurchasesContext _context;

        public LegacySummaryRepository(PurchasesContext context)
        {
            _context = context;
        }

        public (object, object, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>>) Summary(int userId)
        {
            var categories = _context.Category
                .Where(c => c.UserId == userId)
                .Select(c => new { c.Name, Category_id = c.CategoryId.ToString() })
                .ToList();

            var subCategories = _context.Subcategory
                .Join(_context.Category, s => s.CategoryId, c => c.CategoryId, (s, c) => new { s, c.UserId })
                .Where(r => r.UserId == userId)
                .Select(pair => new
                {
                    pair.s.Name,
                    Subcategory_id = pair.s.SubcategoryId.ToString(),
                    Category_id = pair.s.CategoryId.ToString(),
                })
                .ToList();

            var summary =
                (from posting in _context.Posting
                 join subcategory in _context.Subcategory on posting.SubcategoryId equals subcategory.SubcategoryId
                 join category in _context.Category on subcategory.CategoryId equals category.CategoryId
                 where posting.UserId == userId
                 group posting.Amount by new { posting.Date.Year, posting.Date.Month, subcategory.SubcategoryId, category.CategoryId } into g
                 select new {
                     g.Key.Year,
                     g.Key.Month,
                     g.Key.SubcategoryId,
                     g.Key.CategoryId,
                     Sum = g.Sum()
                 } ).ToList();

            var summaryMap = summary
                .GroupBy(s => s.CategoryId).ToDictionary(gc => gc.Key,
                    gc => gc.GroupBy(s => s.SubcategoryId).ToDictionary(gs => gs.Key,
                        gs => gs.GroupBy(s => s.Year).ToDictionary(gy => gy.Key,
                            gy => gy.GroupBy(s => s.Month).ToDictionary(gm => gm.Key, gm => gm.Sum(x => x.Sum)
                            )
                        )
                    )
                );

            return (categories, subCategories, summaryMap);
        }
    }
}
