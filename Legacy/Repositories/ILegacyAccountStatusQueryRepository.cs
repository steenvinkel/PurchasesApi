﻿using Business.Models;
using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacyAccountStatusQueryRepository
    {
        (dynamic, dynamic) MonthlyAccountStatus(int userId);
        MonthlyAccountCategorySums GetAccumulatedCategorySums(int userId);
        Dictionary<MonthAndYear, double> CalculateSummedFortunes(int userId);
        (double StartSum, double EndSum) StartAndEndOfMonthAccountStatusSum(int userId, MonthAndYear monthAndYear);
    }
}
