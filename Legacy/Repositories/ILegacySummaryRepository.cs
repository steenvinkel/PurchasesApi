﻿using System.Collections.Generic;

namespace Legacy.Repositories
{
    public interface ILegacySummaryRepository
    {
        (object, object, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, double>>>>) Summary(int userId);
    }
}