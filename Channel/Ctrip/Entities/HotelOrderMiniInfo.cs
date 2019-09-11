using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Entities
{
    [Serializable]
    public class HotelOrderMiniInfo
    {
        public string AllianceId { get; set; }
        public string Sid { get; set; }
        /// <summary>
        /// 联盟自定义参数，即传入ouid
        /// </summary>
        public string Ouid { get; set; }
        /// <summary>
        /// 携程订单号
        /// </summary>
        public string OrderId { get; set; }
        public string HotelId { get; set; }
        public string HotelName { get; set; }
        /// <summary>
        /// 来源订单，当订单为复制或修改时此值会存在
        /// </summary>
        public string FromOrderId { get; set; }
        public string OrderStatus { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

    }
}
