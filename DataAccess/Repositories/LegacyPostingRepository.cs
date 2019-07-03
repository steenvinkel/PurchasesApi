using DataAccess.Models;
using Legacy.Models;
using Legacy.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacyPostingRepository : ILegacyPostingRepository
    {
        private readonly PurchasesContext _context;

        public LegacyPostingRepository(PurchasesContext purchasesContext)
        {
            _context = purchasesContext;
        }

        public List<LegacyPosting> Get(int userId)
        {
            var postings = from posting in _context.Posting
                           join subcategory in _context.Subcategory
                               on posting.SubcategoryId equals subcategory.SubcategoryId
                           where posting.UserId == userId
                           select new { posting, Description = subcategory.Name };

            return postings.Select(p => new LegacyPosting
            {
                PostingId = p.posting.PostingId,
                Amount = p.posting.Amount,
                Date = p.posting.Date,
                Latitude = p.posting.Latitude,
                Longitude = p.posting.Longitude,
                Accuracy = p.posting.Accuracy,
                Description = p.Description
            }).ToList();
        }
    }
}
