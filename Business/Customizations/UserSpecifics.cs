using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
