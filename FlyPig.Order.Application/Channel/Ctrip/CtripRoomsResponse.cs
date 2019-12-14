using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.Ctrip
{
    public class CtripRoomsResponse : CtBaseResponse
    {
        public List<CtRoomStaticInfos> RoomStaticInfos { get; set; }
    }

    [Serializable]
    public class CtRoomStaticInfos
    {
        public CtRoomTypeInfo RoomTypeInfo { get; set; }
        public List<CtRoomInfos> RoomInfos { get; set; }
    }

    [Serializable]
    public class CtRoomTypeInfo
    {
        public CtSmoking Smoking { get; set; }
        public CtBroadNet BroadNet { get; set; }
        public CtChildLimit ChildLimit { get; set; }
        public List<CtFacilities> Facilities { get; set; }
        public List<CtRoomBedInfos> RoomBedInfos { get; set; }
        public string RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }
        public string StandardRoomType { get; set; }
        public int RoomQuantity { get; set; }
        public int MaxOccupancy { get; set; }
        public string AreaRange { get; set; }
        public string FloorRange { get; set; }
        /// <summary>
        /// 定义物理房型是否有窗：0-无窗；1-部分有窗；2-有窗；4-内窗；5-天窗；6-封闭窗；7-飘窗；-100未知；
        /// </summary>
        public string HasWindow { get; set; }
        public string ExtraBedFee { get; set; }
        public string BathRoomType { get; set; }

        public List<CtRoomInfos> RoomInfos { get; set; }
        public List<CtDescriptions> Descriptions { get; set; }
    }

    [Serializable]
    public class CtRoomLimit
    {
        public string WeeklyIndex { get; set; }
        public string End { get; set; }
        public string Start { get; set; }
    }

    [Serializable]
    public class CtSmoking
    {
        public string HasRoomInNonSmokingArea { get; set; }
        public string HasNonSmokingRoom { get; set; }
        public string HasDeoderizedRoom { get; set; }
        public string NotAllowSmoking { get; set; }
    }

    [Serializable]
    public class CtBroadNet
    {
        public int HasBroadnet { get; set; }
        public string HasWirelessBroadnet { get; set; }
        public string WirelessBroadnetRoom { get; set; }
        public string WirelessBroadnetFee { get; set; }
        public string HasWiredBroadnet { get; set; }
        public string WiredBroadnetRoom { get; set; }
        public string WiredBroadnetFee { get; set; }
        public string BroadnetFeeDetail { get; set; }
        public int WirelessBroadnet { get; set; }
        public int WiredBroadnet { get; set; }
    }

    [Serializable]
    public class CtChildLimit
    {
        public int MaxOccupancy { get; set; }
        public int MaxAge { get; set; }
        public int MinAge { get; set; }
    }
    [Serializable]
    public class CtRoomBedInfos
    {
        public List<CtBedInfo> BedInfo { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }

    [Serializable]
    public class CtBedInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string NumberOfBeds { get; set; }
        public string BedWidth { get; set; }
    }

    [Serializable]
    public class CtRoomInfos
    {
        public CtApplicabilityInfo ApplicabilityInfo { get; set; }
        public CtAreaApplicabilityInfo AreaApplicabilityInfo { get; set; }
        public CtSmoking Smoking { get; set; }
        public CtBroadNet BroadNet { get; set; }
        public List<CtRoomBedInfos> RoomBedInfos { get; set; }
        public CtRoomFGToPPInfo RoomFGToPPInfo { get; set; }
        public List<CtRoomGiftInfo> RoomGiftInfo { get; set; }
        public CtChannelLimit ChannelLimit { get; set; }
        public CtExpressCheckout ExpressCheckout { get; set; }
        public List<CtRoomTags> RoomTags { get; set; }
        public List<CtBookingRules> BookingRules { get; set; }
        public List<CtDescriptions> Descriptions { get; set; }

        public CtRoomLimit RoomLimit { get; set; }
        /// <summary>
        /// 备注：售卖房型是真实售卖的房型，其父类为物理房型。同一个物理房型下，可以存在多个售卖房型。
        /// </summary>
        public string RoomID { get; set; }
        public string RoomName { get; set; }
        public int RoomQuantity { get; set; }
        public int MaxOccupancy { get; set; }
        public string AreaRange { get; set; }
        public string FloorRange { get; set; }
        /// <summary>
        /// 定义该售卖房型是否有窗：0-无窗；1-部分有窗；2-有窗；空-有窗；
        /// </summary>
        public string HasWindow { get; set; }
        public string ExtraBedFee { get; set; }
        public bool IsHourlyRoom { get; set; }
        /// <summary>
        /// 定义该售卖房型是否是直连房型
        /// </summary>
        public bool IsFromAPI { get; set; }
        /// <summary>
        /// 是否展示代理商标签
        /// </summary>
        public bool IsShowAgencyTag { get; set; }
        /// <summary>
        /// /// <summary>
        /// 1-携程可提供发票；2-酒店可提供发票
        /// </summary>
        /// </summary>
        public int InvoiceType { get; set; }

        public string InvoiceMode { get; set; }
        /// <summary>
        /// 酒店是否提供专票
        /// </summary>
        public string IsSupportSpecialInvoice { get; set; }
        public bool ReceiveTextRemark { get; set; }
    }

    [Serializable]
    public class CtExpressCheckout
    {
        public bool IsSupported { get; set; }
        public string DepositRatio { get; set; }
    }

    [Serializable]
    public class CtRoomTags
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Desc { get; set; }
    }
    [Serializable]
    public class CtRoomGiftInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
    [Serializable]
    public class CtChannelLimit
    {
        public bool IsApp { get; set; }
        public bool IsWeb { get; set; }
    }
    [Serializable]
    public class CtRoomFGToPPInfo
    {
        public bool CanFGToPP { get; set; }
    }

    [Serializable]
    public class CtApplicabilityInfo
    {
        public List<int> ApplicabilityIDs { get; set; }
        public string Applicability { get; set; }
        public string OtherDescription { get; set; }
    }
    [Serializable]
    public class CtAreaApplicabilityInfo
    {
        public List<CtDetails> Details { get; set; }
    }
    [Serializable]
    public class CtDetails
    {
        public string ContinentID { get; set; }
        public string ContinentName { get; set; }
        public string RegionID { get; set; }
        public string RegionName { get; set; }
        public int IsApplicative { get; set; }
    }
    [Serializable]
    public class CtBookingRules
    {
        /// <summary>
        /// 提前预订促销
        /// </summary>
        public List<CtBookingOffsets> BookingOffsets { get; set; }
        /// <summary>
        /// 预订间数促销
        /// </summary>
        public CtTotalOccupancy TotalOccupancy { get; set; }
        /// <summary>
        /// 连续入住时长促销
        /// </summary>
        public List<CtLengthsOfStay> LengthsOfStay { get; set; }
        public List<CtMemberLimitInfo> MemberLimitInfo { get; set; }
        public List<CtDiscount> Discount { get; set; }
        /// <summary>
        /// 日期时间限制
        /// </summary>
        public List<CtTimeLimitInfo> TimeLimitInfo { get; set; }
    }

    [Serializable]
    public class CtLengthsOfStay
    {
        /// <summary>
        /// 连续入住时长促销：Max-最大连住时长；Min-最小连住时长；
        /// </summary>
        public string MinMaxType { get; set; }
        /// <summary>
        /// 连续入住的时长数值
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 时长数值的单位：Year-年；Month-月；Week-周；Day-天 (默认)；Hour-小时；Minute-分钟；Second-秒数；
        /// </summary>
        public string TimeUnit { get; set; }
        /// <summary>
        /// 实际入住时长为预设入住时长的倍数时，是否也可享受该促销优惠：True-是；False-否 (默认)。
        /// </summary>
        public bool MustBeMultiple { get; set; }
    }

    [Serializable]
    public class CtBookingOffsets
    {
        /// <summary>
        /// 提前预订促销：Max-最大提前预定限制；Min-最小提前预定限制；
        /// </summary>
        public string MinMaxType { get; set; }
        /// <summary>
        /// 提前的时间数值
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 时间数值的单位，详情如下：
        ////Year-年；
        ////Month-月；
        ////Week-周；
        ////Day-天 (默认)；
        ////Hour-小时；
        ////Minute-分钟；
        ////Second-秒数；
        /// </summary>
        public string TimeUnit { get; set; }
    }

    [Serializable]
    public class CtTotalOccupancy
    {
        /// <summary>
        /// 预订间数促销：最大预定间数
        /// </summary>
        public string Max { get; set; }
        /// <summary>
        /// 预订间数促销：最小预定间数
        /// </summary>
        public string Min { get; set; }
    }

    [Serializable]
    public class CtMemberLimitInfo
    {
        /// <summary>
        /// 是否EDM专享
        /// </summary>
        public bool EDM { get; set; }
        /// <summary>
        /// 是否普通会员专享
        /// </summary>
        public bool General { get; set; }
        /// <summary>
        /// 是否黄金会员专享
        /// </summary>
        public bool Gold { get; set; }
        /// <summary>
        /// 是否铂金会员专享
        /// </summary>
        public bool Platinum { get; set; }
        /// <summary>
        /// 基于会员身份的促销：是否钻石会员专享
        /// </summary>
        public bool Diamond { get; set; }
        /// <summary>
        /// 基于会员身份的促销：是否微信会员专享
        /// </summary>
        public bool WeChat { get; set; }
    }

    [Serializable]
    public class CtDiscount
    {
        public int NightsRequired { get; set; }
        public int NightsDiscounted { get; set; }
        public string DiscountPattern { get; set; }
    }

    [Serializable]
    public class CtTimeLimitInfo
    {
        public List<CtDateRestrictions> DateRestrictions { get; set; }
        public List<CtAvailableDaysOfWeek> AvailableDaysOfWeek { get; set; }

    }
    [Serializable]
    public class CtAvailableDaysOfWeek
    {
        /// <summary>
        /// 星期几限制的类别：Booking-预定；Arrival-到店；StayIn-入住期间；Departure-离店；
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// 从周一到周日，该促销适用于星期几。示例：1110110表示周一、周二、周三、周五、周六适用，周四和周日不适用）备注：存在日期范围和星期
        /// </summary>
        public string WeeklyIndex { get; set; }
    }
    [Serializable]
    public class CtDateRestrictions
    {
        /// <summary>
        /// /// <summary>
        /// 日期时间限制的类别：Booking-预定；Arrival-到店；StayIn-入住期间；Departure-离店；
        /// </summary>
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// 定义相邻节点Start和End的日期时间格式：Date表示仅有日期限制；Time表示仅有时间限制；DateTime表示日期+时间限制；
        /// </summary>
        public string DateType { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        /// <summary>
        /// 是否当日有效(T表示是)
        /// </summary>
        public string IsIntraday { get; set; }
    }

    /************简易*****************/
    [Serializable]
    public class TgCtripRooms
    {
        public List<TgRoomStaticInfos> RoomStaticInfos { get; set; }
    }

    [Serializable]
    public class TgRoomStaticInfos
    {
        public TgRoomTypeInfo RoomTypeInfo { get; set; }
        public List<TgRoomInfos> RoomInfos { get; set; }
    }

    [Serializable]
    public class TgRoomTypeInfo
    {
        public string RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }
        public string HasWindow { get; set; }
        public string MaxOccupancy { get; set; }
        public List<CtRoomBedInfos> RoomBedInfos { get; set; }
    }

    [Serializable]
    public class TgRoomInfos
    {
        public string RoomID { get; set; }
        public string RoomName { get; set; }
        public int MaxOccupancy { get; set; }
        public bool IsHourlyRoom { get; set; }
        /// <summary>
        /// 1-携程可提供发票；2-酒店可提供发票
        /// </summary>
        public int InvoiceType { get; set; }
        public string HasWindow { get; set; }
        public List<CtRoomBedInfos> RoomBedInfos { get; set; }
        public List<CtRoomTags> RoomTags { get; set; }
    }
    [Serializable]
    public class CtFacilities
    {
        public List<CtFacilityItem> FacilityItem { get; set; }
        public string CategoryName { get; set; }
    }
    [Serializable]
    public class CtFacilityItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 可能的值：0-不确定；1-所有房间；2-部分房间
        /// </summary>
        public string Status { get; set; }
    }
    [Serializable]
    public class CtDescriptions
    {
        public string Category { get; set; }
        public string Text { get; set; }
    }
}
