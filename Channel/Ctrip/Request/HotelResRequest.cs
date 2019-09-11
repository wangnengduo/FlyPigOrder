using Ctrip.Common;
using Ctrip.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctrip.Request
{
    public class HotelResRequest
    {
        [RequsetNode("OTA_HotelResRQ")]
        public OTA_HotelResRQ OTA_HotelResRQ { get; set; }
        public HotelResRequest(ApiConfig config) { this.OTA_HotelResRQ = new OTA_HotelResRQ(config); }

    }
    public class OTA_HotelResRQ
    {
        [RequsetNode("OTA_HotelResRQ", "TimeStamp", false)]
        public string TimeStamp { get; set; }
        /// <summary>
        /// //预付房型下单使用2.0版本，即Version="2.0"；现付押金担保下单使用3.0版本，即Version="3.0"；预付分销商收款订单中RatePlanCategory为501或502，预付携程收款订单中RatePlanCategory为508；现付转预付RatePlanCategory为507
        /// </summary>
        [RequsetNode("OTA_HotelResRQ", "Version", false)]
        public string Version { get; set; }
        [RequsetNode("OTA_HotelResRQ", "PrimaryLangID", false)]
        public string PrimaryLangID { get; set; }
        [RequsetNode("OTA_HotelResRQ", "RatePlanCategory", false)]
        public string RatePlanCategory { get; set; }
        [RequsetNode("UniqueID")]
        public List<UniqueID> UniqueIDList { get; set; }
        [RequsetNode("HotelReservations")]
        public HotelReservations HotelReservations { get; set; }


        public OTA_HotelResRQ(ApiConfig config)
        {
            this.TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff+08:00");
            this.Version = "2.0";
            this.PrimaryLangID = "zh";
            //this.RatePlanCategory = "501";
            this.UniqueIDList = new List<UniqueID>()
            {
                //new UniqueID(){ Type="504", ID="100000"},
                new UniqueID(){ Type="28",ID=config.AllianceID},
                new UniqueID(){Type="503",ID=config.SID},
                new UniqueID(){ Type="1",ID=config.UID}
            };
            this.HotelReservations = new HotelReservations();
        }
    }
    public class UniqueID
    {
        /// <summary>
        /// Type属性：ID类型，参考CodeList(UIT) 504 固定为100000 ； 28联盟的ID；503联盟的站点ID；1表示联盟用户在携程的uniqueid；string类型；必填 
        /// </summary>
        [RequsetNode("UniqueID", "Type", false)]
        public string Type { get; set; }
        [RequsetNode("UniqueID", "ID", false)]
        public string ID { get; set; }
    }
    public class HotelReservations
    {
        [RequsetNode("HotelReservation", false)]
        public List<HotelReservation> HotelReservationList { get; set; }
        public HotelReservations() { this.HotelReservationList = new List<HotelReservation>(); }
    }
    public class HotelReservation
    {
        [RequsetNode("RoomStays")]
        public RoomStays RoomStays { get; set; }
        [RequsetNode("ResGuests")]
        public ResGuests ResGuests { get; set; }
        [RequsetNode("ResGlobalInfo")]
        public ResGlobalInfo ResGlobalInfo { get; set; }
        public HotelReservation()
        {
            this.RoomStays = new RoomStays();
            this.ResGuests = new ResGuests();
            this.ResGlobalInfo = new ResGlobalInfo();
        }
    }
    public class RoomStays
    {
        [RequsetNode("RoomStay")]
        public List<RoomStay> RoomStayList { get; set; }
        public RoomStays() { this.RoomStayList = new List<RoomStay>(); }
    }
    public class RoomStay
    {
        [RequsetNode("RoomTypes")]
        public RoomTypes RoomTypes { get; set; }
        [RequsetNode("RatePlans")]
        public RatePlans RatePlans { get; set; }
        [RequsetNode("BasicPropertyInfo", "HotelCode", false)]
        public string HotelCode { get; set; }
        public RoomStay()
        {
            this.RoomTypes = new RoomTypes();
            this.RatePlans = new RatePlans();
        }
    }
    public class RoomTypes
    {
        [RequsetNode("RoomType", "NumberOfUnits", false)]
        public int NumberOfUnits { get; set; }
    }
    public class RatePlans
    {
        [RequsetNode("RatePlan", "RatePlanCode", false)]
        public string RatePlanCode { get; set; }

        [RequsetNode("RatePlan", "RatePlanCategory", false)]
        public string RatePlanCategory { get; set; }
    }
    public class ResGuests
    {
        [RequsetNode("ResGuest")]
        public List<ResGuest> ResGuestList { get; set; }
        public ResGuests() { this.ResGuestList = new List<ResGuest>(); }
    }
    public class ResGuest
    {
        //[RequsetNode("ResGuest", "ArrivalTime", true)]
        public string ArrivalTime { get; set; }
        [RequsetNode("Profiles")]
        public Profiles Profiles { get; set; }
        [RequsetNode("TPA_Extensions")]
        public TPA_Extensions_LateArrivalTime TPA_Extensions { get; set; }
        public ResGuest() { this.Profiles = new Profiles(); this.TPA_Extensions = new TPA_Extensions_LateArrivalTime(); }
    }



    public class Profiles
    {
        [RequsetNode("ProfileInfo")]
        public ProfileInfo ProfileInfo { get; set; }
        public Profiles() { this.ProfileInfo = new ProfileInfo(); }
    }
    public class ProfileInfo
    {
        [RequsetNode("Profile")]
        public Profile Profile { get; set; }
        public ProfileInfo() { this.Profile = new Profile(); }

    }
    public class Profile
    {
        [RequsetNode("Customer")]
        public Customer Customer { get; set; }
        public Profile() { this.Customer = new Customer(); }
    }
    public class Customer
    {
        [RequsetNode("PersonName")]
        public List<PersonName> PersonNameList { get; set; }
        [RequsetNode("ContactPerson")]
        public ContactPerson ContactPerson { get; set; }
        public Customer() { this.PersonNameList = new List<PersonName>(); this.ContactPerson = new ContactPerson(); }
    }
    public class PersonName
    {
        [RequsetNode("Surname")]
        public string Surname { get; set; }
    }
    public class ContactPerson
    {
        [RequsetNode("ContactPerson", "ContactType", false)]
        public string ContactType { get; set; }
        [RequsetNode("PersonName")]
        public PersonName PersonName { get; set; }
        [RequsetNode("Telephone", "PhoneNumber", false)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 电话类型，参考CodeList(PTT)
        /// </summary>
        [RequsetNode("Telephone", "PhoneTechType", false)]
        public string PhoneTechType { get; set; }
        /// <summary>
        /// 预付房型必填项
        /// </summary>
        [RequsetNode("Email")]
        public string Email { get; set; }
        public ContactPerson() { this.ContactType = "tel"; this.PhoneTechType = "5"; this.Email = "fame@128uu.com"; }

    }
    public class ResGlobalInfo
    {
        [RequsetNode("GuestCounts")]
        public GuestCounts GuestCounts { get; set; }
        /// <summary>
        /// 入住开始时间
        /// </summary>
        [RequsetNode("TimeSpan", "Start", false)]
        public string StartDate { get; set; }
        /// <summary>
        /// 入住结束时间
        /// </summary>
        [RequsetNode("TimeSpan", "End", false)]
        public string EndDate { get; set; }
        [RequsetNode("SpecialRequests")]
        public SpecialRequests SpecialRequests { get; set; }
        /// <summary>
        /// 存款支付：本节点用于指明担保金的支付情况。如果本订单已做支付担保，则传入担保制度代码和已支付金额。担保制度验证不通过不能做预订。 担保制度在可订检查和价格计划列表里已提供给分销商，由分销商负责组合担保制度并收入担保费用，可空
        /// </summary>
        [RequsetNode("DepositPayments")]
        public DepositPayments DepositPayments { get; set; }
        [RequsetNode("Total")]
        public Total Total { get; set; }

        [RequsetNode("TPA_Extensions")]
        public TPA_Extensions_TotalCost TPA_Extensions { get; set; }
        

        [RequsetNode("CancelPenalties")]
        public CancelPenalties CancelPenalties { get; set; }

        public ResGlobalInfo()
        {
            this.GuestCounts = new GuestCounts();
            this.SpecialRequests = new SpecialRequests();
            this.Total = new Total();
            this.TPA_Extensions = new TPA_Extensions_TotalCost { CurrencyCode="CNY" };
        }
    }
    public class SpecialRequests
    {
        [RequsetNode("SpecialRequest")]
        public SpecialRequest SpecialRequest { get; set; }
        public SpecialRequests() { this.SpecialRequest = new SpecialRequest(); }
    }
    public class SpecialRequest
    {
        [RequsetNode("Text")]
        public string Text { get; set; }
    }
    public class DepositPayments
    {
        [RequsetNode("GuaranteePayment")]
        public GuaranteePayment GuaranteePayment { get; set; }
        public DepositPayments() { this.GuaranteePayment = new GuaranteePayment(); }
    }
    public class GuaranteePayment
    {
        [RequsetNode("GuaranteePayment", "GuaranteeType", true)]
        public string GuaranteeType { get; set; }
        [RequsetNode("AcceptedPayments")]
        public AcceptedPayments AcceptedPayments { get; set; }
        /// <summary>
        /// 接收到的担保金额
        /// </summary>
        [RequsetNode("AmountPercent", false)]
        public AmountPercent AmountPercent { get; set; }
        [RequsetNode("TPA_Extensions")]
        public TPA_Extensions_OtherCurrency TPA_Extensions { get; set; }
    }
    public class AcceptedPayments
    {
        [RequsetNode("AcceptedPayment")]
        public AcceptedPayment AcceptedPayment { get; set; }
        public AcceptedPayments() { this.AcceptedPayment = new AcceptedPayment(); }
    }
    public class AcceptedPayment
    {
        [RequsetNode("PaymentCard")]
        public PaymentCard PaymentCard { get; set; }

    }
    public class PaymentCard
    {
        /// <summary>
        /// 磁卡类型；可空
        /// </summary>
        [RequsetNode("PaymentCard", "CardType", true)]
        public string CardType { get; set; }
        /// <summary>
        /// 信用卡号；string类型；可空
        /// </summary>
        [RequsetNode("PaymentCard", "CardNumber", true)]
        public string CardNumber { get; set; }
        /// <summary>
        /// 串号信用卡背后的3位数字，如没有可以不录入；string类型；可空
        /// </summary>
        [RequsetNode("PaymentCard", "SeriesCode", true)]
        public string SeriesCode { get; set; }
        /// <summary>
        ///  ExpireDat和EffectiveDate属性：卡片有效日期，这两个值一致，都是卡面的日期；datetime类型；可空
        /// </summary>
        [RequsetNode("PaymentCard", "EffectiveDate", true)]
        public string EffectiveDate { get; set; }
        [RequsetNode("PaymentCard", "ExpireDate", true)]
        public string ExpireDate { get; set; }
        /// <summary>
        /// 持卡人姓名；可空
        /// </summary>
        [RequsetNode("CardHolderName")]
        public string CardHolderName { get; set; }
        /// <summary>
        /// 持卡人身份证号,可空
        /// </summary>
        [RequsetNode("CardHolderIDCard")]
        public string CardHolderIDCard { get; set; }
        /// <summary>
        /// 银行ID
        /// </summary>
        [RequsetNode("CardBankID")]
        public string CardBankID { get; set; }
        /// <summary>
        /// 银行预留手机号；可空
        /// </summary>
        [RequsetNode("MobilePhone")]
        public string MobilePhone { get; set; }

    }
    public class AmountPercent
    {
        [RequsetNode("AmountPercent", "Amount", false)]
        public decimal Amount { get; set; }
        /// <summary>
        /// CurrencyCode属性：货币单位；可空
        /// </summary>
        [RequsetNode("AmountPercent", "CurrencyCode", true)]
        public string CurrencyCode { get; set; }
    }
    public class TPA_Extensions_OtherCurrency
    {
        [RequsetNode("OtherCurrency", "Amount", false)]
        public decimal Amount { get; set; }
        [RequsetNode("OtherCurrency", "CurrencyCode", true)]
        public string CurrencyCode { get; set; }
    }

    public class TPA_Extensions_TotalCost
    {
        [RequsetNode("TotalCost", "AmountBeforeTax", false)]
        public decimal AmountBeforeTax { get; set; }

        [RequsetNode("TotalCost", "CurrencyCode", false)]
        public string CurrencyCode { get; set; }
    }


    public class Total
    {
        [RequsetNode("Total", "AmountBeforeTax", false)]
        public decimal AmountBeforeTax { get; set; }
        [RequsetNode("Total", "CurrencyCode", false)]
        public string CurrencyCode { get; set; }

        public Total() { this.CurrencyCode = "CNY"; }

    }
    public class CancelPenalties
    {
        [RequsetNode("CancelPenalty")]
        public List<CancelPenalty> CancelPenaltyList { get; set; }
        public CancelPenalties() { this.CancelPenaltyList = new List<CancelPenalty>(); }
    }
    public class CancelPenalty
    {
        [RequsetNode("CancelPenalty", "Start", false)]
        public string StartTime { get; set; }
        [RequsetNode("CancelPenalty", "End", false)]
        public string EndTime { get; set; }
        [RequsetNode("AmountPercent")]
        public AmountPercent AmountPercent { get; set; }
    }

}
