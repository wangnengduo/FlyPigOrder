using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class NumExtension
    {

        public static decimal ToTaoBaoPrice(this decimal price)
        {
            if (price != 0)
            {
                price = price * 100;
            }
            return price;
        }


    }
}
