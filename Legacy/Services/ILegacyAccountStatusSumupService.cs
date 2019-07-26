using Business.Models;
using Legacy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legacy.Services
{
    public interface ILegacyAccountStatusSumupService
    {
        Dictionary<MonthAndYear, LegacyAccountStatusSums> GetSumup(int userId);
    }
}
