using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.DaDuShi.Entity
{
    public class RatePlanConfrimJsonEt
    {
        /// <summary>
        /// 酒店代码
        /// </summary>
        public string HotelCode { get; set; }
        /// <summary>
        /// 价格计划代码
        /// </summary>
        public string RatePlanCode { get; set; }
        /// <summary>
        /// 房型代码
        /// </summary>
        public string RoomTypeCode { get; set; }
        /// <summary>
        /// 入住时间
        /// </summary>
        public string CheckIn { get; set; }
        /// <summary>
        /// 离店时间
        /// </summary>
        public string CheckOut { get; set; }
        /// <summary>
        /// 房间数量
        /// </summary>
        public int NumberOfUnits { get; set; }
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
