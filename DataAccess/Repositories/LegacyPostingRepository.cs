using DataAccess.Models;
using Legacy.Models;
using Legacy.Repositories;
using System;
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
            var postings = (from posting in _context.PostingForUser(userId)
                           join subcategory in _context.Subcategory
                               on posting.SubcategoryId equals subcategory.SubcategoryId into ps
                           from sub in ps.DefaultIfEmpty()
                           orderby posting.CreatedOn descending
                           select new { posting, Description = sub.Name  }
                           ).Take(200).ToList();

            return postings.Select(p => new LegacyPosting
            {
                Posting_id = p.posting.PostingId,
                Amount = p.posting.Amount,
                Date = p.posting.Date,
                Latitude = p.posting.Latitude,
                Longitude = p.posting.Longitude,
                Accuracy = p.posting.Accuracy,
                Description = p.Description ?? p.posting.Description
            }).ToList();
        }

        public LegacyPosting Put(LegacyPosting posting, int userId)
        {
            var existingPosting = _context.PostingForUser(userId).FirstOrDefault(p => p.PostingId == posting.Posting_id);
            if (existingPosting == null)
            {
                throw new Exception($"Posting ({posting.Posting_id}) does not exists");
            }

            var subcategoryId = GetSubcategoryIdFromName(userId, posting.Description);
            existingPosting.Amount = posting.Amount;
            existingPosting.Description = subcategoryId == null ? posting.Description : null;
            existingPosting.SubcategoryId = subcategoryId;
            existingPosting.Date = posting.Date;
            existingPosting.Longitude = posting.Longitude;
            existingPosting.Latitude = posting.Latitude;
            existingPosting.Accuracy = posting.Accuracy;

            _context.SaveChanges();

            return MapToLegacyPosting(posting.Description, existingPosting);

        }

        public LegacyPosting Post(LegacyPosting posting, int userId)
        {
            var subcategoryId = GetSubcategoryIdFromName(userId, posting.Description);

            var newPosting = new Posting
            {
                Amount = posting.Amount,
                Description = subcategoryId == null ? posting.Description : null,
                Date = posting.Date.Date,
                Longitude = posting.Longitude,
                Latitude = posting.Latitude,
                Accuracy = posting.Accuracy,
                UserId = userId,
                CreatedOn = DateTime.Now,
                SubcategoryId = subcategoryId,
            };

            _context.Posting.Add(newPosting);
            _context.SaveChanges();

            return MapToLegacyPosting(posting.Description, newPosting);
        }

        private static LegacyPosting MapToLegacyPosting(string description, Posting posting)
        {
            return new LegacyPosting
            {
                Posting_id = posting.PostingId,
                Amount = posting.Amount,
                Date = posting.Date.Date,
                Longitude = posting.Longitude,
                Latitude = posting.Latitude,
                Accuracy = posting.Accuracy,
                Description = posting.SubcategoryId != null ? description : posting.Description
            };
        }

        private int? GetSubcategoryIdFromName(int userId, string name)
        {
            var subcategoryId = _context.Subcategory
                .Join(_context.Category, s => s.CategoryId, c => c.CategoryId, (s, c) => new { s, c.UserId })
                .Where(r => r.UserId == userId)
                .Where(r => r.s.Name == name)
                .Select(r => r.s.SubcategoryId)
                .ToList();
                ;

            return subcategoryId.Cast<int?>()
                .FirstOrDefault();
        }
    }
}
