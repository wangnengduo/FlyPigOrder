using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaDuShi.Response
{
    public class OrderInfoJsonEt
    {
        /// <summary>
        /// 订单状态
        /// </summary>
        public string ReservationStatus { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 大都市产品系统订单ID
        /// </summary>
        public string ProductReservationId { get; set; }
        /// <summary>
        /// 大都市结算系统订单ID
        /// </summary>
        public string ErpReservationId { get; set; }
        /// <summary>
        /// 订单截止退变更时间
        /// </summary>
        public string CancelTime { get; set; }
        /// <summary>
        /// 渠道方订单号
        /// </summary>
        public string DistributorReservationId { get; set; }
    }
}