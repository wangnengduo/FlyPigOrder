using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class OrderQueryResponse : BaseResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public OrderQueryResult result { get; set; }

    }

    public class OrderQueryResult
    {
        /// <summary>
        /// 
        /// </summary>
        public List<OrderInfosItem> orderInfos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 成功
        /// </summary>
        public string desc { get; set; }
    }

    public class BaseInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int mtOrderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int goodsId { get; set; }
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
        public DateTime createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int orderStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? goodsType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fixRoom { get; set; }
    }

    public class AptInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int mtOrderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime checkInTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime checkOutTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string arriveTime { get; set; }
        /// <summary>
        /// 请安排靠近楼梯的房间
        /// </summary>
        public string comment { get; set; }
        /// <summary>
        /// 标间全日房
        /// </summary>
        public string roomName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int roomId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int roomCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int hotelId { get; set; }

        public string poiName { get; set; }


        public string personNames { get; set; }

        public string contactName { get; set; }

        public string contactPhone { get; set; }
    }

    public class OrderInfosItem
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseInfo baseInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AptInfo aptInfo { get; set; }

        public List<RoomNightInfo> roomNights { get; set; }

        public List<OrderRefundInfo> refundInfos { get; set; }

    }

    public class RoomNightInfo
    {

        public DateTime bizDate { get; set; }

        public int appointmentStatus { get; set; }

        public int payStatus { get; set; }

        public decimal sellPrice { get; set; }

        public decimal subPrice { get; set; }
    }

    public class OrderRefundInfo
    {
        public int RefundId { get; set; }

        public int RefundPrice { get; set; }

        public int RefundSettlePrice { get; set; }

        public string RefundTime { get; set; }
    }

}
