using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Dto
{
    public class BookingCheckOutDto
    {
        /// <summary>
        /// 是否可预定
        /// </summary>
        public bool IsBook { get; set; }

        public string Message { get; set; }

        public List<RateQuotasPrice> DayPrice { get; set; }

    }


    [Serializable]
    public class RateQuotasPrice
    {
        public RateQuotasPrice()
        {
        }
        public RateQuotasPrice(string date, int price, int quota)
        {
            this.Date = date;
            this.Quota = quota;
            this.Price = price;
        }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("quota")]
        public int Quota { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
