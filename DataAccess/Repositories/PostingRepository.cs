using Business.Repositories;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class PostingRepository(PurchasesContext context) : IPostingRepository
    {
        private readonly PurchasesContext _context = context;

        public IEnumerable<Business.Models.Posting> Get200Descending(int userId)
        {
            var postings = _context.PostingForUser(userId).OrderByDescending(p => p.UpdatedOn).Take(200).ToList();

            return postings.Select(Map);
        }

        public IEnumerable<Business.Models.Posting> GetAllForSubcategory(int userId, int subcategoryId)
        {
            var postings = _context.PostingForUser(userId).Where(p => p.SubcategoryId == subcategoryId).ToList();

            return postings.Select(Map);
        }

        public Business.Models.Posting Add(int userId, Business.Models.Posting posting)
        {
            var newPosting = new Posting
            {
                UserId = userId,
                SubcategoryId = posting.SubcategoryId,
                Amount = posting.Amount,
                Date = posting.Date.Date,
                CreatedOn = DateTime.Now
            };

            _context.Posting.Add(newPosting);
            _context.SaveChanges();
            return Map(newPosting);
        }

        public Business.Models.Posting Update(int userId, Business.Models.Posting posting)
        {
            var updatedPosting = _context.PostingForUser(userId).Single(p => p.PostingId == posting.PostingId);
            updatedPosting.SubcategoryId = posting.SubcategoryId;
            updatedPosting.Amount = posting.Amount;
            updatedPosting.Date = posting.Date.Date;
            _context.SaveChanges();

            return Map(updatedPosting);
        }

        private Business.Models.Posting Map(Posting posting)
        {
            if (!posting.SubcategoryId.HasValue)
            {
                throw new ArgumentNullException(nameof(posting.SubcategoryId));
            }
            return new Business.Models.Posting(posting.PostingId, posting.UserId, posting.SubcategoryId.Value, posting.Amount, posting.Date);     
        }
    }
}
