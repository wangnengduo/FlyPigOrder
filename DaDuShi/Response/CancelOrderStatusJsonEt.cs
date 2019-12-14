using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaDuShi.Response
{
    public class CancelOrderStatusJsonEt
    {
        /// <summary>
        /// 大都市产品系统订单编号
        /// </summary>
        public string ProductReservationId { get; set; }
        /// <summary>
        /// 渠道商订单ID
        /// </summary>
        public string DistributorReservationId { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string Status { get; set; }
    }
}