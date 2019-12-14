using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaDuShi.Request
{
    public class OrderRequestJsonEt
    {
        /// <summary>
        /// 大都市产品系统订单编号
        /// </summary>
        public string ProductReservationId { get; set; }
        /// <summary>
        /// 渠道商订单ID
        /// </summary>
        public string DistributorReservationId { get; set; }
    }
}