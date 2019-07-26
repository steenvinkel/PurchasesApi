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

        public static double CreateExtraLine(int userId, int year, int month, double @in, double tax)
        {
            var extra = 0.0;
            if (userId == 1)
            {
                var procent50 = 19590.0;
                if (year > 2015 || (month > 6 && year == 2015))
                {
                    procent50 = 16800.0 + 10600;
                }
                if (year > 2016 || (month > 9 && year == 2016))
                {
                    procent50 = 19200.0 + 13900;
                }
                if (year > 2016 || (month > 11 && year == 2016))
                {
                    procent50 = 16000.0 + 26000;
                }
                if (year > 2017 || (month > 11 && year == 2017))
                {
                    procent50 = (@in - tax) / 2 + tax;
                }
                extra = procent50;
            }

            return extra;
        }

        public static double GetPensionRate(int userId, MonthAndYear monthAndYear)
        {
            if (!Rules.IsJcpSpecific(userId))
            {
                return 0;
            }

            if (monthAndYear.IsEarlierThan(new MonthAndYear(2016, 10)))
            {
                return 0;
            }
            else if (monthAndYear.IsEarlierThan(new MonthAndYear(2018,8)))
            {
                return 0.0525;
            }
            return 0.0625;
        }
    }
}
