using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Entity
{
    public class MTPriceInfo
    {
        public bool CanBook { get; set; }

        public double Total_SalePrice { get; set; }

        public double Total_SumSubPrice { get; set; }

        public Dictionary<DateTime, double> Pricedic { get; set; }
        public Dictionary<DateTime, double> SubPricedic { get; set; }

    }
}
