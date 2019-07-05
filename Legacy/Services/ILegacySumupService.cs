using System.Collections.Generic;

namespace Legacy.Services
{
    public interface ILegacySumupService
    {
        List<object> Sumup(int userId);
    }
}
