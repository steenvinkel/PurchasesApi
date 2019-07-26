using System;
using System.Collections.Generic;
using System.Text;
using Business.Models;

namespace Business.Customizations
{
    public static class UserSpecifics
    {
        public static int GetTaxCategoryId(int userId)
        {
            var taxCategoryId = 0;
            if (Rules.IsJcpSpecific(userId))
            {
                taxCategoryId = 15;
            }

            return taxCategoryId;
        }

        public static bool ShouldDailyPurchaseMonthBeRemoved(MonthAndYear monthAndYear, int userId)
        {
            return Rules.IsJcpSpecific(userId) && monthAndYear.IsEarlierThan(new MonthAndYear(2014, 9));
        }
    }
}
