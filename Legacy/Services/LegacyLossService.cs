using System;
using System.Linq;
using Business.Models;
using Business.Repositories;
using Legacy.Repositories;

namespace Legacy.Services
{
    public class LegacyLossService(ILegacyPostingQueryRepository postingQueryRepository,
        ILegacyAccountStatusQueryRepository accountStatusQueryRepository,
        ISubCategoryRepository subCategoryRepository,
        IPostingRepository postingRepository) : ILegacyLossService
    {
        private readonly ILegacyPostingQueryRepository _postingQueryRepository = postingQueryRepository;
        private readonly ILegacyAccountStatusQueryRepository _accountStatusQueryRepository = accountStatusQueryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository = subCategoryRepository;
        private readonly IPostingRepository _postingRepository = postingRepository;

        public void CalculateLoss(int userId, MonthAndYear monthAndYear)
        {
            var (startSum, endSum) = _accountStatusQueryRepository.StartAndEndOfMonthAccountStatusSum(userId, monthAndYear);
            var monthlyChange = _postingQueryRepository.GetMonthlyChange(userId, monthAndYear);

            var loss = Math.Round(monthlyChange + startSum - endSum, 2);

            UpdateLoss(userId, monthAndYear, loss);
        }

        private void UpdateLoss(int userId, MonthAndYear monthAndYear, decimal loss)
        {
            var lossSubCategoryId = _subCategoryRepository.GetList(userId)
                .Where(s => s.Name == Business.Constants.SubCategoryProperties.Name.Loss)
                .Select(s => s.SubCategoryId)
                .Single();

            var lossPosting = _postingRepository.GetAllForSubcategory(userId, lossSubCategoryId)
                .Where(posting => posting.Date.Year == monthAndYear.Year)
                .Where(posting => posting.Date.Month == monthAndYear.Month)
                .SingleOrDefault();

            if (lossPosting == null)
            {
                lossPosting = new Posting(0, userId, lossSubCategoryId, loss, monthAndYear.LastDayOfMonth());

                _postingRepository.Add(userId, lossPosting);
            }
            else
            {
                lossPosting.Amount = loss;

                _postingRepository.Update(userId, lossPosting);
            }
        }
    }
}
