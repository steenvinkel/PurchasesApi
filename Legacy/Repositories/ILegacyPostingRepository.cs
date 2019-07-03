using Legacy.Models;
using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacyPostingRepository
    {
        List<LegacyPosting> Get(int userId);
    }
}
