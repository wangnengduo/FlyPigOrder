using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.MeiTuan.Entity.mtPriceCacheEntity
{
    public class CommonRate
    {
        //public CommonRate();

        public decimal AddBedRate { get; set; }
        public int Bonus { get; set; }
        public decimal Commission { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime Date { get; set; }
        public string HotelID { get; set; }
        public string InvStatusCode { get; set; }
        public decimal MemberRate { get; set; }
        public string MyHotelID { get; set; }
        public string RatePlanID { get; set; }
        public decimal RetailRate { get; set; }
        public string RoomTypeID { get; set; }
    }
}
