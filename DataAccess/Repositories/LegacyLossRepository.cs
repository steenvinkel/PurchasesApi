using Business.Models;
using DataAccess.Models;
using Legacy.Repositories;
using System;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacyLossRepository : ILegacyLossRepository
    {
        private readonly PurchasesContext _context;
        private readonly ILegacyPostingQueryRepository _postingQueryRepository;

        public LegacyLossRepository(PurchasesContext context, ILegacyPostingQueryRepository postingQueryRepository)
        {
            _context = context;
            _postingQueryRepository = postingQueryRepository;
        }

        public void UpdateLoss(int userId, MonthAndYear monthAndYear, double loss)
        {
            int lossSubcategoryId = _postingQueryRepository.GetLossSubCategoryId(userId);

            Posting lossPosting = GetExistingLossPosting(userId, monthAndYear, lossSubcategoryId);

            if (lossPosting == null)
            {
                lossPosting = CreateNewLossPosting(userId, monthAndYear, lossSubcategoryId);
                _context.Posting.Add(lossPosting);
            }

            lossPosting.Amount = loss;
            lossPosting.CreatedOn = DateTime.Now;

            _context.SaveChanges();
        }

        private static Posting CreateNewLossPosting(int userId, MonthAndYear monthAndYear, int lossSubcategoryId)
        {
            return new Posting
            {
                UserId = userId,
                SubcategoryId = lossSubcategoryId,
                Description = null,
                Date = monthAndYear.LastDayOfMonth(),
                Latitude = null,
                Longitude = null,
                Accuracy = null,
            };
        }

        private Posting GetExistingLossPosting(int userId, MonthAndYear monthAndYear, int lossSubcategoryId)
        {
            return _context.PostingForUser(userId)
                .Where(posting => posting.Date.Year == monthAndYear.Year)
                .Where(posting => posting.Date.Month == monthAndYear.Month)
                .Where(posting => posting.SubcategoryId == lossSubcategoryId)
                .SingleOrDefault();
        }
    }
}
