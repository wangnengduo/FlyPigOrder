using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    /// <summary>
    /// 创建订单xml
    /// </summary>
    [Serializable]
    public class BookRQ : TaoBaoXml
    {
        [XmlIgnore]
        public override RequestXmlType XmlType
        {
            get { return RequestXmlType.BookRQ; }
        }

        [XmlElement]
        public long TaoBaoOrderId { get; set; }
        [XmlElement]
        public long TaoBaoHotelId { get; set; }
        [XmlElement]
        public string HotelId { get; set; }
        [XmlElement]
        public long TaoBaoRoomTypeId { get; set; }
        [XmlElement]
        public string RoomTypeId { get; set; }
        [XmlElement]
        public long TaoBaoRatePlanId { get; set; }
        [XmlElement]
        public string RatePlanCode { get; set; }
        [XmlElement]
        public long TaoBaoGid { get; set; }
        [XmlElement]
        public DateTime CheckIn { get; set; }
        [XmlElement]
        public DateTime CheckOut { get; set; }
        [XmlElement]
        public bool HourRent { get; set; }

        [XmlElement]
        public string EarliestArriveTime { get; set; }
        [XmlElement]
        public string LatestArriveTime { get; set; }
        [XmlElement]
        public int RoomNum { get; set; }
        [XmlElement]
        public int PriceType { get; set; }

        [XmlElement]
        public int InventoryType { get; set; }

        [XmlElement]
        public long TotalPrice { get; set; }
        /// <summary>
        /// 1预付5面付
        /// </summary>
        [XmlElement]
        public int PaymentType { get; set; }

        [XmlElement]
        public string Currency { get; set; }
        [XmlElement]
        public string ContactName { get; set; }

        [XmlElement]
        public string ContactTel { get; set; }
        [XmlElement]
        public string ContactEmail { get; set; }


        [XmlArray]
        [XmlArrayItem("DailyInfo")]
        public List<DailyInfo> DailyInfos { get; set; }

        [XmlArray]
        [XmlArrayItem("DailyInfo")]
        public List<DailyInfo> OriDailyInfos { get; set; }

        [XmlArray]
        [XmlArrayItem("DailyInfo")]
        public List<DailyInfo> DisDailyInfos { get; set; }

        [XmlElement]
        public TravelInfo TravelInfo { get; set; }

        [XmlArray]
        [XmlArrayItem("OrderGuest")]
        public List<OrderGuest> OrderGuests { get; set; }
        [XmlElement]
        public string Comment { get; set; }

        [XmlArray]
        [XmlArrayItem("VoucherInfo")]
        public List<VoucherInfo> VoucherInfos { get; set; }


        /// <summary>
        /// 0无担保1 首晚担保2 全额担保
        /// </summary>
        [XmlElement]
        public int GuaranteeType { get; set; }

        [XmlElement]
        public string MemberCardNo { get; set; }

        [XmlElement]
        public string AlipayTradeNo { get; set; }
        [XmlElement]
        public string CertificationCode { get; set; }

        [XmlElement]
        public XCreditCardInfo CreditCardInfo { get; set; }
        [XmlElement]
        public XInvoiceInfo InvoiceInfo { get; set; }

        [XmlElement]
        public string Extensions { get; set; }


    }

    [Serializable]
    public class TravelInfo
    {
        [XmlElement]
        public int OrderType { get; set; }
        [XmlElement]
        public string Company { get; set; }
    }


    /// <summary>
    /// 每日价格
    /// </summary>
    [Serializable]
    public class DailyInfo
    {
        [XmlElement]
        public string Day { get; set; }
        [XmlElement]
        public long Price { get; set; }
    }
    /// <summary>
    /// 入住人信息
    /// </summary>
    [Serializable]
    public class OrderGuest
    {
        [XmlElement]
        public string Name { get; set; }
        /// <summary>
        /// 房间序号
        /// </summary>
        [XmlElement]
        public string RoomPos { get; set; }
    }

    [Serializable]
    public class ExtensionsItem
    {
        public string guarantee_amount { get; set; }
        public string countryOfPassport { get; set; }
        public string hotel_order_tbosser { get; set; }
        public string firstBookingRatePlan { get; set; }
        public string guarantee_url { get; set; }
        public string guarantee_type { get; set; }
        public string hbsOrderWirelessSource { get; set; }
        public string taobaoGuaranteeAmount { get; set; }
        public string receiptTitle { get; set; }
    }

    [Serializable]
    public class XCreditCardInfo
    {
        public string CardCode { get; set; }
        public string CardHolderName { get; set; }
        public string ExpirationDate { get; set; }
        public string CardNumber { get; set; }
    }

    [Serializable]
    public class XInvoiceInfo
    {
        public int NeedInvoice { get; set; }
        public int EarllyPrepare { get; set; }
        public string SubmitTime { get; set; }
        public string WantTime { get; set; }
        public string PostType { get; set; }
        public string InvoiceType { get; set; }
        public string Comment { get; set; }
        public string InvoiceTitle { get; set; }
        public string CompanyTel { get; set; }
        public string CompanyTax { get; set; }
        public string RegisterAddress { get; set; }
        public string Bank { get; set; }
        public string BankAccount { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverMobile { get; set; }

    }

    public class VoucherInfo
    {
        public int VoucherPomotionAmt { get; set; }

        public string VoucherPomotionDesc { get; set; }

        public string VoucherRuleDesc { get; set; }

        public int VoucherNum { get; set; }

        public int PaidFee { get; set; }
    }
}
