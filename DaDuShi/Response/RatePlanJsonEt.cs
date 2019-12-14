using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaDuShi.Response
{
    public class RatePlanJsonEt
    {
        /// <summary>
        /// 房型code
        /// </summary>
        public string RoomTypeCode { get; set; }
        /// <summary>
        /// 房型名称
        /// </summary>
        public string RoomTypeName { get; set; }
        /// <summary>
        /// 价格计划code
        /// </summary>
        public string RatePlanCode { get; set; }
        /// <summary>
        /// 价格计划名称
        /// </summary>
        public string RatePlanName { get; set; }
        /// <summary>
        /// 当前币种  目前统一转为CNY
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        public string PaymentType { get; set; }
        /// <summary>
        /// 促销规则-连续入住N天及以上优惠，值为0时表示没有此促销。
        /// </summary>
        public int LastDays { get; set; }
        /// <summary>
        /// 促销规则-提前N天预订优惠，值为0时表示没有此促销。
        /// </summary>
        public int AdvanceDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AdvanceHours { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaymentPolicies { get; set; }
        /// <summary>
        /// 具体房型和价格计划上的截止退变更日期。
        /// </summary>
        public DateTime? StopCancelDate { get; set; } 
        /// <summary>
        /// 具体房型和价格计划上的截止退变更日期（单位：分钟）
        /// </summary>
        public int RTStopCancelTime { get; set; }
        /// <summary>
        /// 具体房型和价格计划上的退变更原则描述。
        /// </summary>
        public string RateCancelPolicy { get; set; }
        /// <summary>
        /// 具体房型和价格计划上的是否即订即保信息。  是:表示该价格的房型即订即保
        /// (交了定金即锁定)
        /// </summary>
        public string RateNotUpdateable { get; set; }
        /// <summary>
        /// 早餐数量
        /// -2  含早(根据入住人数匹配)
        /// -1  不定
        /// 0   不含早
        /// 1   单早
        /// 2   双早
        /// 3   三早
        /// 4   四早
        /// </summary>
        public int RTBreakfast { get; set; }
        /// <summary>
        /// 床型名称
        /// </summary>
        public string BedTypeName { get; set; }
        /// <summary>
        /// 保留房提前预定时间
        /// </summary>
        public string RoomTypeUseBeforeTime { get; set; }

        /// <summary>
        /// 价格集合
        /// </summary>
        public List<RateJsonEt> RateList { get; set; }
    }
}