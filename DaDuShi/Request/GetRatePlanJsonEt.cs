﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaDuShi.Request
{
    public class GetRatePlanJsonEt
    {
        /// <summary>
        /// 酒店代码
        /// </summary>
        public string HotelCode { get; set; }
        /// <summary>
        /// 入住时间
        /// </summary>
        public string CheckIn { get; set; }
        /// <summary>
        /// 离店时间
        /// </summary>
        public string CheckOut { get; set; }
        /// <summary>
        /// 成人数
        /// </summary>
        public int AdultCount { get; set; }
        /// <summary>
        /// 儿童数
        /// </summary>
        public int ChildCount { get; set; }
    }
}