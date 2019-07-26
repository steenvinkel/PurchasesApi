using Business.Constants;
using Business.Models;
using DataAccess.Models;
using Legacy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Repositories
{
    public class LegacyLossRepository : ILegacyLossRepository
    {
        private readonly PurchasesContext _context;

        public LegacyLossRepository(PurchasesContext context)
        {
            _context = context;
        }

        public void UpdateLoss(int userId, MonthAndYear monthAndYear, double loss)
        {
            int lossSubcategoryId = GetLossSubCategoryId(userId);

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
            return _context.Posting
                .Where(posting => posting.UserId == userId)
                .Where(posting => posting.Date.Year == monthAndYear.Year)
                .Where(posting => posting.Date.Month == monthAndYear.Month)
                .Where(posting => posting.SubcategoryId == lossSubcategoryId)
                .SingleOrDefault();
        }

        private int GetLossSubCategoryId(int userId)
        {
            return (from S in _context.Subcategory
                    join C in _context.Category on S.CategoryId equals C.CategoryId
                    where C.Name == CategoryProperties.Name.Loss && C.UserId == userId
                    select S.SubcategoryId).Single();
        }

        public (double, double, double) GetMonthlyValues(int userId, MonthAndYear monthAndYear)
        {
            var monthlyAccountStatuses = _context.AccountStatus
                .Where(accountStatus => accountStatus.UserId == userId)
                .GroupBy(accountStatus => new { accountStatus.Date.Year, accountStatus.Date.Month })
                .Select(pair => new { pair.Key.Year, pair.Key.Month, Sum = pair.Sum(x => x.Amount) })
                .ToDictionary(x => new MonthAndYear(x.Year, x.Month), x => x.Sum);

            var monthlyChange = (from P in _context.Posting
                                join S in _context.Subcategory on P.SubcategoryId equals S.SubcategoryId
                                join C in _context.Category on S.CategoryId equals C.CategoryId
                                where P.UserId == userId
                                    && P.Date.Year == monthAndYear.Year
                                    && P.Date.Month == monthAndYear.Month
                                    && C.Name != CategoryProperties.Name.Loss
                                select C.Type == CategoryProperties.Type.In ? P.Amount : -P.Amount)
                                .Sum();

            var endSum = monthlyAccountStatuses[monthAndYear];
            var startSum = monthlyAccountStatuses[monthAndYear.PreviousMonth()];

            return (startSum, monthlyChange, endSum);
        }
    }
}
