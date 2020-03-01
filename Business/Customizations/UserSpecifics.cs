using System;
using System.Collections.Generic;
using System.Text;
using Business.Models;

namespace Business.Customizations
{
    public static class UserSpecifics
    {
        public static List<int> GetTaxSubcategoryIds(int userId)
        {
            if (Rules.IsJcpSpecific(userId))
            {
                return new List<int> { 77, 78, 88 };
            }

            return new List<int>();
        }

        public static bool ShouldDailyPurchaseMonthBeRemoved(MonthAndYear monthAndYear, int userId)
        {
            return Rules.IsJcpSpecific(userId) && monthAndYear.IsEarlierThan(new MonthAndYear(2014, 9));
        }

        public static double CreateExtraLine(int userId, MonthAndYear monthAndYear, double @in, double tax)
        {
            if (Rules.IsJcpSpecific(userId))
            {
                if (monthAndYear.IsLaterThan((2017,11)))
                {
                    return (@in - tax) / 2 + tax;
                }
                if (monthAndYear.IsLaterThan((2016, 11)))
                {
                    return 16000.0 + 26000;
                }
                if (monthAndYear.IsLaterThan((2016,9)))
                {
                    return 19200.0 + 13900;
                }
                if (monthAndYear.IsLaterThan((2015,6)))
                {
                    return 16800.0 + 10600;
                }
                return 19590.0;
            }

            return 0.0;
        }

        public static bool ShouldCalculateSelfPaidPension(int userId)
        {
            return Rules.IsJcpSpecific(userId);
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

        public static bool HideNegativeInWithoutPension(int userId)
        {
            return Rules.IsJcpSpecific(userId);
        }

        public static int GetSalerySubcategoryId(int userId)
        {
            if (Rules.IsJcpSpecific(userId))
            {
                return 2;
            }

            return 0;
        }
    }
}
