using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaDuShi.Response
{
    public class OrderQueryJsonEt
    {
        /// <summary>
        /// 大都市产品系统订单ID
        /// </summary>
        public string ProductReservationId { get; set; }
        /// <summary>
        /// 渠道方订单号
        /// </summary>
        public string DistributorReservationId { get; set; }
        /// <summary>
        /// 大都市结算系统订单ID
        /// </summary>
        public string ERPReservationId { get; set; }
        /// <summary>
        /// 大都市结算系统订单确认号
        /// </summary>
        public string ConfirmNo { get; set; }
        /// <summary>
        /// 订单状态，取值如下：
        /// 1.	Failed
        /// 2.	Confirmed
        /// 3.	Pending
        /// 4.	Canceled
        /// </summary>
        public string ReservationStatus { get; set; }
        /// <summary>
        /// 订单总价
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}