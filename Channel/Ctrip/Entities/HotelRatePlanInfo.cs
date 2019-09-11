using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtripOld.Entity
{
    //[Serializable]
    //public class HotelRatePlanInfo
    //{

    //    public DateTime TimeStamp { get; set; }
    //    public string Version { get; set; }
    //    public string PrimaryLangID { get; set; }
    //    public string HotelCode { get; set; }
    //    public List<RatePlanInfo> RatePlans { get; set; }
    //}
    [Serializable]
    public class RatePlanInfo
    {
        public string TimeUnit { get; set; }
        /// <summary>
        /// 价格计划代码
        /// </summary>
        public string RatePlanCode { get; set; }
        /// <summary>
        /// 价格计划类型代码，参考CodeList (RTC),RatePlanCategory=501和502为预付的价格计划，RatePlanCategory=14和16现付的价格计划。
        /// </summary>
        public string RatePlanCategory { get; set; }
        /// <summary>
        /// 是否代理价
        /// </summary>
        public bool IsCommissionable { get; set; }
        /// <summary>
        /// 是否直连
        /// </summary>
        public bool RateReturn { get; set; }
        /// <summary>
        /// 市场代码，参考CodeList (MKC)，63为全部人群适用
        /// </summary>
        public string MarketCode { get; set; }
        
        /// <summary>
        /// 预订规则列表，注意目前接口下单支持提前预定促销、住几天以上促销、住几送几促销，这三种促销，可以有下面的规则中任意一个也可为空
        /// </summary>
        //public List<object> BookingRules { get; set; }

        /// <summary>
        /// 提前预定天数
        /// </summary>
        public int MinAdvancedBookingOffset { get; set; }
        /// <summary>
        /// 最大提前预订时间差
        /// </summary>
        public int MaxAdvancedBookingOffset { get; set; }
        /// <summary>
        /// 住几天以上促销
        /// </summary>
        public int LengthOfStay { get; set; }
        /// <summary>
        /// 适用的会员等级
        /// </summary>
        public List<string> Viewerships { get; set; }
        /// <summary>
        /// 特定价格计划范围内的每日价列表
        /// </summary>
        public List<Rate> Rates { get; set; }
        /// <summary>
        /// 礼品礼盒促销等Offer信息
        /// </summary>
        public List<Offer> Offers { get; set; }

        public string InvCode { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public string BedTypeCode { get; set; }
        public string Net { get; set; }


        public string BookingCode { get; set; }
        public string RatePlanID { get; set; }
        /// <summary>
        /// 最少预定数
        /// </summary>
        public int MinTotalOccupancy { get; set; }
        public int MaxTotalOccupancy { get; set; }

        public string ProductName { get; set; }
        /// <summary>
        /// 宾客
        /// </summary>
        public string Applicability { get; set; }
        /// <summary>
        /// 1:携程开票，2：酒店开票
        /// </summary>
        public int InvoiceTargetType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsHourRoom { get; set; }
        /// <summary>
        /// 代理商编号
        /// </summary>
        public string VendorID { get; set; }
    }


    [Serializable]
    public class Rate
    {
        /// <summary>
        /// 剩余房间数
        /// </summary>
        public int NumberOfUnits { get; set; }
        /// <summary>
        /// 价格开始时间
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// 价格结束时间
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// 是否即时确认
        /// </summary>
        public bool IsInstantConfirm { get; set; }
        /// <summary>
        /// open可售状态，onrequest 房源紧张,close表示不可售
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 按客人数量的基础价格列表
        /// </summary>
        public List<BaseByGuestAmt> BaseByGuestAmts { get; set; }
        /// <summary>
        /// 对于港澳台及海外酒店，列出外币的价格
        /// </summary>
        public OtherCurrency OtherCurrency { get; set; }
        /// <summary>
        /// 额外费用列表：比如宽带费/加床费/加早餐费
        /// </summary>
        public List<Fee> Fees { get; set; }
        /// <summary>
        /// 担保制度列表，担保金额等于当日房价
        /// </summary>
        public List<GuaranteePolicy> GuaranteePolicies { get;set;}
        /// <summary>
        /// 取消制度列表
        /// </summary>
        public List<CancelPenalty> CancelPolicies { get; set; }
        /// <summary>
        /// 是否含早
        /// </summary>
        public bool Breakfast { get; set; }
        /// <summary>
        /// 早餐的数量
        /// </summary>
        public int NumberOfBreakfast { get; set; }
        /// <summary>
        /// 附加信息 促销信息
        /// </summary>
        public List<RebatePromotion> RebatePromotions { get; set; }
        /// <summary>
        /// PayChange属性：现转预FTP，预转现PTF
        /// </summary>
        public string PayChange { get; set; }
        /// <summary>
        /// 酒店预订时间限制
        /// </summary>
        public DateTime LaterReserveTime { get; set; }
    }
    [Serializable]
    public class BaseByGuestAmt
    {
        /// <summary>
        /// 价格不含税价
        /// </summary>
        public decimal AmountBeforeTax { get; set; }
        public string CurrencyCode { get; set; }
        public int NumberOfGuests { get; set; }
        /// <summary>
        /// 门市价
        /// </summary>
        public decimal ListPrice { get; set; }

    }
    [Serializable]
    public class OtherCurrency
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
    [Serializable]
    public class Fee
    {
        /// <summary>
        /// 费用类型代码，参考CodeList(FTT),38	Rollaway fee(加床)，1001 另加早餐费，1002 宽带费
        /// </summary>
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 扣款单位:如每房每晚/每人,参考CodeList(CHG)
        /// </summary>
        public string ChargeUnit { get; set; }
        /// <summary>
        /// 费用描述
        /// </summary>
        public string Description { get; set; }
    }
    [Serializable]
    public class GuaranteePolicy
    {
        /// <summary>
        /// 担保类型代码，参考CodeList(RGC)   1	峰时担保 2	全额担保 3	超时担保 4	一律担保 5	手机担保
        /// </summary>
        public string GuaranteeCode { get; set; }
        /// <summary>
        /// 在此时间之前不需要担保
        /// </summary>
        public DateTime HoldTime { get; set; }

    }
    [Serializable]
    public class CancelPenalty
    {
        /// <summary>
        /// start表示了最迟的取消时间，在这个时间前取消不需要扣除罚金,之后就需要扣除罚金
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Start属性：开始时间；End属性：结束时间，表示在这个时间段取消是需要扣除罚金
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// 罚金金额=担保金额=当日房价
        /// </summary>
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 对于港澳台及海外酒店，列出外币的价格
        /// </summary>
        public OtherCurrency OtherCurrency { get; set; }
    }
    [Serializable]
    public class RebatePromotion
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartPeriod { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndPeriod { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ProgramName { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 501 限额抵用券 /502 非限额抵用券/503 需要消费券的返现/504 不需要消费券的返现/505 酒店游票任我住/506 酒店游票任我游/507 酒店游票任我行
        /// </summary>
        public string Code { get; set; }

        public string Description { get; set; }
    }
    [Serializable]
    public class Offer
    {
        /// <summary>
        /// 礼盒代码
        /// </summary>
        public string OfferCode { get;set;}
        /// <summary>
        /// 礼盒的入住时间段限制。（如有多个日期段，则输出多条。类似价格变化接口的合并方式。）
        /// </summary>
        public List<OfferRule> OfferRules { get; set; }

        public string OfferDescription { get; set; }
        /// <summary>
        /// 打折(住几送几)
        /// </summary>
        public Discount Discount { get; set; }
    }
    [Serializable]
    public class OfferRule
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string xStart { get; set; }
        public string xEnd { get; set; }
        public string RestrictionDateCode { get; set; }
        public string RestrictionType { get;set; }
    }
    [Serializable]
    public class Discount
    {
        /// <summary>
        /// 满足要求的间夜数
        /// </summary>
        public int NightsRequired { get; set; }
        /// <summary>
        /// 免费送的间夜数
        /// </summary>
        public int NightsDiscounted { get; set; }
        /// <summary>
        /// 收费/免费房夜的数据格式
        /// </summary>
        public string DiscountPattern { get; set; }
    }



}
