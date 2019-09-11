using Ctrip.Common;
using Ctrip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Request
{
    public class HotelResSubmitRequest : IBaseRequest<HotelResSubmitResponse>
    {
        [RequsetNode("OTA_HotelResSubmitRQ")]
        public OTA_HotelResSubmitRQ OTA_HotelResSubmitRQ { get; set; }

        public string Method => "OTA_HotelResSubmit";

        public string Url => "/Hotel/OTA_HotelResSubmit.asmx";

        public HotelResSubmitRequest() { this.OTA_HotelResSubmitRQ = new OTA_HotelResSubmitRQ(); }

    }
    public class OTA_HotelResSubmitRQ
    {
        [RequsetNode("OTA_HotelResSubmitRQ", "TransactionIdentifier", false)]
        public string TransactionIdentifier { get; set; }
        [RequsetNode("OTA_HotelResSubmitRQ", "TimeStamp", false)]
        public string TimeStamp { get; set; }
        [RequsetNode("OTA_HotelResSubmitRQ", "Version", false)]
        public string Version { get; set; }
        [RequsetNode("OTA_HotelResSubmitRQ", "PrimaryLangID", false)]
        public string PrimaryLangID { get; set; }
        [RequsetNode("ReservationPayment")]
        public ReservationPayment ReservationPayment { get; set; }
        public OTA_HotelResSubmitRQ()
        {
            this.TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff+08:00");
            this.Version = "1.0";
            this.PrimaryLangID = "zh";
            this.ReservationPayment = new ReservationPayment();
        }
    }
    public class ReservationPayment
    {
        [RequsetNode("ReservationID", "Type", false)]
        public string ReservationID_Type { get; set; }
        [RequsetNode("ReservationID", "ID", false)]
        public string ReservationID_ID { get; set; }
        [RequsetNode("PaymentDetail")]
        public PaymentDetail PaymentDetail { get; set; }

        [RequsetNode("Invoice")]
        public Invoice Invoice { get; set; }

        public ReservationPayment()
        {
            this.ReservationID_Type = "501";
            this.PaymentDetail = new PaymentDetail();
            this.Invoice = new Invoice { CurrencyCode = "CNY", Type = "Invoice" };
        }
    }

    public class Invoice
    {
        [RequsetNode("Amount", "Type", false)]
        public string Type { get; set; }
        [RequsetNode("Amount", "Amount", false)]
        public decimal Amount { get; set; }
        [RequsetNode("Amount", "CurrencyCode", false)]
        public string CurrencyCode { get; set; }
    }

    public class PaymentDetail
    {
        [RequsetNode("PaymentDetail", "PaymentType", false)]
        public string PaymentType { get; set; }
        [RequsetNode("PaymentDetail", "GuaranteeIndicator", false)]
        public bool GuaranteeIndicator { get; set; }
        [RequsetNode("PaymentDetail", "GuaranteeTypeCode", false)]
        public string GuaranteeTypeCode { get; set; }
        [RequsetNode("ChannelAccount", "ChannelAccountIndicator", false)]
        public bool ChannelAccountIndicator { get; set; }
        [RequsetNode("PaymentAmount", "CurrencyCode", false)]
        public string CurrencyCode { get; set; }
        [RequsetNode("PaymentAmount", "Amount", false)]
        public string Amount { get; set; }
        public PaymentDetail()
        {
            this.PaymentType = "9";
            this.GuaranteeTypeCode = "4";
            this.ChannelAccountIndicator = true;
            this.CurrencyCode = "CNY";
        }
    }
}
