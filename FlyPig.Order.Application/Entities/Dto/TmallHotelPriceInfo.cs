using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Dto
{
    /// <summary>
    /// 天猫酒店价格信息
    /// </summary>
    public class TmallHotelPriceInfo
    {
        public string HotelName { get; set; }

        public string RoomName { get; set; }

        public string RatePlanName { get; set; }

        public int PaymentType { get; set; }

        public string CurrencyCode { get; set; }

        public decimal? CurrencyPrice { get; set; }

        public string PriceStr { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public decimal STotalPrice { get; set; }

        /// <summary>
        /// 底价
        /// </summary>

        public decimal DatePrice { get; set; }
    }
}
