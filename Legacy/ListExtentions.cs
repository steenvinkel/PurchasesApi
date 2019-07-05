using Legacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legacy
{
    public static class ListExtentions
    {
        public static void RemoveFirstMonthInFirstYear(this List<MonthlyTypeSum> list)
        {
            var itemIndex = -1;
            var year = int.MaxValue;
            var month = int.MaxValue;
            for (var i = 0; i < list.Count(); i ++)
            {
                if (list[i].Year < year && list[i].Month < month)
                {
                    itemIndex = i;
                    year = list[i].Year;
                    month = list[i].Month;
                }
            }

            if (itemIndex >= 0)
            {
                list.RemoveAt(itemIndex);
            }
        }
    }
}
