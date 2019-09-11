
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctrip.Entities
{
    [Serializable]
    public class ResRetrieveInfo
    {
        public HeaderInfo Header { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Version { get; set; }
        public string PrimaryLangID { get; set; }


    }
    [Serializable]
    public class HotelReservation
    {
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 预订创建者ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 上次修改时间
        /// </summary>
        public DateTime LastModifyDateTime { get; set; }
        /// <summary>
        /// 上次修改源ID
        /// </summary>
        public string LastModifierID { get; set; }
        /// <summary>                    
        /// 订单状态，参考CodeList(RVS)  /S 成交/C 取消/P 确认供应商/I 确认货位/G 确认客户/W 处理中
        /// </summary>                   
        public string ResStatus { get; set; }
        /// <summary>
        /// 订单大状态值，参数如下：1）process:处理中  2）confirm:已确认 3）cancel：取消 4）success：成交。只要获取到这个字段的值是“confirm”的状态，就表示订单确认..
        /// 注意：1）cancel的状态包含两种情形：a)确认失败 b）客人确实未入住2）success状态包含如下情形：a)用户成功离店 b)担保订单noshow已扣款..
        /// </summary>
        public string OrderStatus { get; set; }

        public string CtripOrderId { get; set; }
        /// <summary>
        /// 订几间房
        /// </summary>
        public int NumberOfUnits { get; set; }
        public string RoomTypeCode { get; set; }
        public string RatePlanCode { get; set; }
        public List<AdditionalDetail> AdditionalDetails { get; set; }

        public string HotelCode { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        /// <summary>
        /// BillingCode属性：支付方式，FG:现付PP:预付
        /// </summary>
        public string BillingCode { get; set; }
        public string ArrivalTime { get; set; }
        public List<string> CustomerNames { get; set; }
        public string ContactName { get; set; }
        public string ContactType { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string LateArrivalTime { get; set; }

        /// <summary>
        /// 客人数量是否对应每间房，False 表示所有房间加起来一共住这么多客
        /// </summary>
        public bool IsPerRoom { get; set; }
        public int GuestCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 客人特殊要求
        /// </summary>
        public string SpecialRequest { get; set; }
        /// <summary>
        /// 税前总价,订单总价
        /// </summary>
        public decimal AmountBeforeTax { get; set; }
        public string CurrencyCode { get; set; }
        /// <summary>
        /// 其它币种总价
        /// </summary>
        public decimal OtherAmount { get; set; }
        public string OtherCurrencyCode { get; set; }
        /// <summary>
        /// 担保支付总价
        /// </summary>
        public decimal NoShowAmount { get; set; }
        public string NoShowCurrencyCode { get; set; }

        public List<HotelReservationID> HotelReservationIDs { get; set; }


        /// <summary>
        ///  日夜审结果，参考CodeList(DNA), 1 入住, 2 离店, 3 noshow
        /// </summary>
        public string DayNightAudit { get; set; }

        public List<OrderTag> OrderTags;

    }

    public class OrderTag
    {
        public string Code { get; set; }

        public string Value { get; set; }

    }






    [Serializable]
    public class AdditionalDetail
    {
        /// <summary>
        /// 描述信息所属大类，参考CodeList（ADT）
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 小类别，目前有促销和礼盒两类，用户可以根据这个类别在房型名称前用图标标识出是否有促销或礼盒，参考参考CodeList（HOC）
        /// </summary>
        public string Code { get; set; }
        public List<string> Texts { get; set; }
    }
    [Serializable]
    public class HotelReservationID
    {
        /// <summary>
        /// Resid_type=”501” 表示原订单号，如果这个节点中 cancelReason 和cancellaionDate,那么说明这个订单被取消了
        /// 判断是否有Resid_type=”507” 的节点，如果有，则其中的 resID_value就是新订单的ID号。用这个新的订单ID号，去获取指定的订单，就可以完成新老订单的关联了
        /// </summary>
        public string ResID_Type { get; set; }
        public string ResID_Value { get; set; }
        public string CancelReason { get; set; }
        public DateTime CancellationDate { get; set; }
        //501	携程订单号                     506	变更房价流水号
        //502	酒店订单确认号                 507	携程订单号(关联订单号)
        //503	分销联盟用户子帐号ID           508	支付信息保存服务返回ID
        //504	订单流水号（由接口调用者生成） 509	补单原订单号
        //505	变更房态房量流水号

    }

}
