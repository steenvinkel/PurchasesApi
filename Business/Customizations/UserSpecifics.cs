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
    }
}
