using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaDuShi.Response
{
    public class HotelInfoJsonEt
    {
        /// <summary>
        /// 酒店代码
        /// </summary>
        public string HotelCode { get; set; }
        /// <summary>
        /// 当天预订截止时间（精确到分钟，例如17:00）
        /// </summary>
        public string LastOrderTime { get; set; }
        public string Inform { get; set; }

        /// <summary>
        /// 价格计划集合
        /// </summary>
        public List<RatePlanJsonEt> RatePlanList { get; set; }
    }
}