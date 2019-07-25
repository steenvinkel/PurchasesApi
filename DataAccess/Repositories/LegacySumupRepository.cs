using Business.Customizations;
using DataAccess.Models;
using Legacy.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacySumupRepository : ILegacySumupRepository
    {
        public readonly PurchasesContext _context;

        public LegacySumupRepository(PurchasesContext context)
        {
            _context = context;
        }

        public List<MonthlyTypeSum> Sumup(int userId)
        {
            var taxCategoryId = UserSpecifics.GetTaxCategoryId(userId);

            var inAndOut = (from p in _context.Posting
                       join s in _context.Subcategory on p.SubcategoryId equals s.SubcategoryId
                       join c in _context.Category on s.CategoryId equals c.CategoryId
                       where p.UserId == userId
                       group p.Amount by new { p.Date.Year, p.Date.Month, c.Type } into g
                       select new {
                           g.Key.Year,
                           g.Key.Month,
                           g.Key.Type,
                           Sum = Math.Round(g.Sum(), 2)
                       }).ToList();

            var invest = (
                from a in _context.AccountStatus
                group a.Amount by new { a.Date.Year, a.Date.Month } into g
                select new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Type = "invest",
                    Sum = Math.Round(g.Sum() * 0.02 / 12, 2)
                }).ToList();

            var tax = (from p in _context.Posting
                       join s in _context.Subcategory on p.SubcategoryId equals s.SubcategoryId
                       join c in _context.Category on s.CategoryId equals c.CategoryId
                       where p.UserId == userId && c.Type == "out" && c.CategoryId == taxCategoryId
                       group p.Amount by new { p.Date.Year, p.Date.Month } into g
                       select new
                       {
                           g.Key.Year,
                           g.Key.Month,
                           Type = "tax",
                           Sum = Math.Round(g.Sum(), 2)
                       }
                ).ToList();

            return inAndOut.Union(tax).Union(invest).Select(x => new MonthlyTypeSum
            {
                Year = x.Year,
                Month = x.Month,
                Type = x.Type,
                Sum = x.Sum,
            }).ToList();
        }
    }
}
