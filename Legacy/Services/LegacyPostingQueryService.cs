using Business.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Legacy.Services
{
    public class LegacyPostingQueryService : ILegacyPostingQueryService
    {
        private readonly ISubCategoryRepository _subCategoryRepository;

        public LegacyPostingQueryService(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }

        public List<string> GetAllSubCategoryNames(int userId)
        {
            var subcategories = _subCategoryRepository.GetList(userId);

            var subcategoryNames = subcategories.Select(s => s.Name).ToList();

            return subcategoryNames;
        }
    }
}
