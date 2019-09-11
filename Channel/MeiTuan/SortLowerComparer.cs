using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan
{
    public class SortLowerComparer : IComparer<string>
    {
        public int Compare(String x, String y)
        {
            String xLower = x.ToLower();
            String yLower = y.ToLower();
            return xLower.CompareTo(yLower);
        }
    }
}
