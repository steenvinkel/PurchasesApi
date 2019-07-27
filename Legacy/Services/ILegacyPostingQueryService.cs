using System.Collections.Generic;

namespace Legacy.Services
{
    public interface ILegacyPostingQueryService
    {
        List<string> GetAllSubCategoryNames(int userId);
    }
}
