using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    public class RateQuotasPrice
    {
        public RateQuotasPrice()
        {
        }

        public RateQuotasPrice(string date, int price, int quota)
        {
            this.date = date;
            this.quota = quota;
            this.price = price;
        }


        public string date { get; set; }
        public int quota { get; set; }
        public decimal price { get; set; }
    }
}
