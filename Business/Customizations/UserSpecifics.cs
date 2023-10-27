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
    }
}
