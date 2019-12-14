using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.Ctrip.ctripPriceCacheEntity
{
    public class CtripStaticRoomRateResponse : CtBaseResponse
    {
        public List<CtStsRoomPriceItems> RoomPriceItems { get; set; }
    }

    [Serializable]
    public class CtStsRoomPriceItems
    {
        public CtRoomStatusInfo RoomStatusInfo { get; set; }
        public List<CtsRoomPriceInfos> RoomPriceInfos { get; set; }
        public string RoomID { get; set; }
        public string EffectiveDate { get; set; }
    }
    [Serializable]
    public class CtRoomStatusInfo
    {
        public CtHoldDeadline HoldDeadline { get; set; }
        public List<CtCancelTimeLimitInfo> CancelTimeLimitInfo { get; set; }
        public CtReserveTimeLimitInfo ReserveTimeLimitInfo { get; set; }
        /// <summary>
        /// G-表示良好；L或S-表示不可超、紧张；N-表示满房；U-表示未知；
        /// </summary>
        public string RoomStatus { get; set; }
        public string AvailableQuantity { get; set; }
        public bool IsInstantConfirm { get; set; }
        public string GuaranteeCode { get; set; }
    }

    [Serializable]
    public class CtCancelTimeLimitInfo
    {
        /// <summary>
        /// 仅用于预付、现付担保房型。FirstDay-扣首日；Full-扣全额；
        /// </summary>
        public string DeductType { get; set; }
        /// <summary>
        /// Prepay-若房型为预付房型，则该时间表示最晚取消时间；Guarantee-若房型为现付需担保房型，则该时间表示最晚取消时间；若房型为现付、
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 最晚取消时间
        /// </summary>
        public string Time { get; set; }
    }

    [Serializable]
    public class CtsRoomPriceInfos
    {
        public CtMealInfo MealInfo { get; set; }
        /// <summary>
        /// 支付类型，PP-预付，FG-到付
        /// </summary>
        public string PayType { get; set; }
        public List<CtPrices> Prices { get; set; }

    }
}
