using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class OrderBookingRequest : IBaseRequest<OrderBookingResponse>
    {

        /// <summary>
        /// 
        /// </summary>
        public long hotelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long goodsId { get; set; }
        /// <summary>
        /// 张三,李四
        /// </summary>
        public string personNames { get; set; }
        /// <summary>
        /// 张三
        /// </summary>
        public string contactName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string contactPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string arriveDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string checkinDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string checkoutDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int roomNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int totalPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int settlePrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string distributorOrderId { get; set; }
        /// <summary>
        /// 请安排靠近楼梯的房间
        /// </summary>
        public string comment { get; set; }

        public int needInvoice { get; set; }

        public string Method => "hotel.order.booking";
    }
}
