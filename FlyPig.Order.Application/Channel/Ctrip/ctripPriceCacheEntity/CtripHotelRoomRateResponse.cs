using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.Ctrip.ctripPriceCacheEntity
{
    public class CtripHotelRoomRateResponse : CtBaseResponse
    {
        public List<CtRoomPriceItems> RoomPriceItems { get; set; }
    }

    [Serializable]
    public class CtRoomPriceItems
    {
        public List<CtRoomPriceInfos> RoomPriceInfos { get; set; }
        public string RoomTypeID { get; set; }
    }

    [Serializable]
    public class CtRoomPriceInfos
    {
        public List<CtCancelPolicyInfos> CancelPolicyInfos { get; set; }
        /// <summary>
        /// 最晚预订时间
        /// </summary>
        public CtReserveTimeLimitInfo ReserveTimeLimitInfo { get; set; }
        public CtHoldDeadline HoldDeadline { get; set; }
        public List<CtPriceInfos> PriceInfos { get; set; }
        public string RoomID { get; set; }
        public string RoomName { get; set; }
    }

    [Serializable]
    public class CtReserveTimeLimitInfo
    {
        /// <summary>
        /// 允许的最晚下单时间（非入住时间）
        /// </summary>
        public string LatestReserveTime { get; set; }
        /// <summary>
        /// 保留房最晚下单时间（非入住时间）
        /// </summary>
        public string LatestReserveTimeForBlockedRoom { get; set; }
    }

    [Serializable]
    public class CtCancelPolicyInfos
    {
        public List<CtPenaltyAmount> PenaltyAmount { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }

    [Serializable]
    public class CtPenaltyAmount
    {
        public string Type { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
    }

    [Serializable]
    public class CtHoldDeadline
    {
        /// <summary>
        /// 超时担保时间节点。若最晚到店时间超过该保留时间，则需要提供担保。备注：适用于现付类房型且该房型的担保政策为超时担保；
        /// </summary>
        public string HoldTime { get; set; }
    }

    [Serializable]
    public class CtPriceInfos
    {
        public List<CtPrices> Prices { get; set; }
        public List<CtDailyPrices> DailyPrices { get; set; }
        public List<CtTaxes> Taxes { get; set; }
        public List<CtFees> Fees { get; set; }
        /// <summary>
        /// PP-预付；FG-现付；
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 该售卖房型的所属分类，详情：501-标准预付房型;502-促销预付房型;16-标准到付房型;14-促销到付房型;
        /// </summary>
        public string RatePlanCategory { get; set; }
        /// <summary>
        /// 售卖房型是否可预订，true-可预订，false-不可预订
        /// </summary>
        public bool IsCanReserve { get; set; }
        /// <summary>
        /// 售卖房型是否需担保，true-需担保，false-不需担保
        /// </summary>
        public bool IsGuarantee { get; set; }
        /// <summary>
        /// 售卖房型是否可立即确认(仅代表订单确认速度，分销商仍需通过相关接口同步携程订单状态)。True-该房型为立即确认房型，false-该房型非立即确认房型
        /// </summary>
        public bool IsInstantConfirm { get; set; }
        /// <summary>
        /// 可定房量，10间以内显示真实房量，大于10间显示 ”10+”
        /// </summary>
        public string RemainingRooms { get; set; }

        public string BookingCode { get; set; }

        public string RatePlanID { get; set; }
    }

    [Serializable]
    public class CtPrices
    {
        /// <summary>
        /// 定义相邻节点的金额为原币种还是自定义币种。目前仅两种取值：DisplayCurrency; OriginalCurrency;
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 每日税前价
        /// </summary>
        public decimal ExclusiveAmount { get; set; }
        /// <summary>
        /// 每日税后价
        /// </summary>
        public decimal InclusiveAmount { get; set; }
        public string Currency { get; set; }
        /// <summary>
        /// 落地数据才有值
        /// </summary>
        public decimal Amount { get; set; }
    }

    [Serializable]
    public class CtDailyPrices
    {
        public CtMealInfo MealInfo { get; set; }
        public List<CtPrices> Prices { get; set; }
        public List<CtCashbacks> Cashbacks { get; set; }
        public List<CtDiscountDetailInfos> DiscountDetailInfos { get; set; }
        public string EffectiveDate { get; set; }
        /// <summary>
        /// 担保类型：1表示峰时担保；2表示全额担保；3表示超时担保；4表示一律担保；5表示手机担保；
        /// </summary>
        public string GuaranteeCode { get; set; }
    }

    [Serializable]
    public class CtMealInfo
    {
        public int NumberOfBreakfast { get; set; }
        public int NumberOfLunch { get; set; }
        public int NumberOfDinner { get; set; }
    }

    [Serializable]
    public class CtCashbacks
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    [Serializable]
    public class CtDiscountDetailInfos
    {

    }

    [Serializable]
    public class CtTaxes
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    [Serializable]
    public class CtFees
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
