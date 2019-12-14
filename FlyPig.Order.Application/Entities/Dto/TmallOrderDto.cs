using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Dto
{
    /// <summary>
    /// 天猫订单Dto
    /// </summary>
    public class TmallOrderDto
    {
        public long aId { get; set; }
        public string orderNO { get; set; }
        public long taoBaoOrderId { get; set; }
        public long TaoBaoHotelId { get; set; }
        public long TaoBaoRoomTypeId { get; set; }
        public long TaoBaoRatePlanId { get; set; }
        public long taoBaoGId { get; set; }
        public string hotelID { get; set; }
        public string roomID { get; set; }
        public string ratePlanID { get; set; }
        public string RatePlanCode { get; set; }
        public string hotelName { get; set; }
        public string roomName { get; set; }
        public string ratePlanName { get; set; }
        public string title { get; set; }
        public int userID { get; set; }
        public string contactTel { get; set; }
        public string contactName { get; set; }
        public string guestName { get; set; }
        public string guestMobile { get; set; }
        public int roomNum { get; set; }
        public System.DateTime checkInDate { get; set; }
        public System.DateTime checkOutDate { get; set; }
        public System.DateTime orderTime { get; set; }
        public string payType { get; set; }
        public string earliestArriveTime { get; set; }
        public string lastArriveTime { get; set; }
        public string tradeStatus { get; set; }
        public string logisticsStatus { get; set; }
        public string refundStatus { get; set; }
        public Nullable<int> state { get; set; }
        public string priceStr { get; set; }
        public decimal totalPrice { get; set; }
        public Nullable<int> sRoomNum { get; set; }
        public Nullable<System.DateTime> sCheckInDate { get; set; }
        public Nullable<System.DateTime> sCheckOutDate { get; set; }
        public string sPriceStr { get; set; }
        public Nullable<decimal> sTotalPrice { get; set; }
        public string remark { get; set; }
        public string caozuo { get; set; }
        public string source { get; set; }
        public string ip { get; set; }
        public string sourceOrderID { get; set; }
        public decimal taobaoTotalPrice { get; set; }
        public string alipayTradeNo { get; set; }
        public Nullable<decimal> alipayPay { get; set; }
        public string DailyInfoPrice { get; set; }
        public string comment { get; set; }
        public short orderType { get; set; }
        public short shopType { get; set; }
        public Nullable<System.DateTime> modifiedTime { get; set; }
        public System.DateTime sentfaxtime { get; set; }
        public string confirmfaxNo { get; set; }
        public string cancelfaxNo { get; set; }
        public string changefaxNo { get; set; }
        public string originalbook { get; set; }
        public string originalname { get; set; }
        public Nullable<int> isfinish { get; set; }
        public Nullable<System.DateTime> confirmspan { get; set; }
        public Nullable<int> peifu { get; set; }
        public string extensions { get; set; }
        public Nullable<int> IsLastOrder { get; set; }
        public Nullable<int> signid { get; set; }
        public Nullable<int> Refuse { get; set; }
        public string handleAccount { get; set; }
        public decimal commision { get; set; }
        public decimal bonus { get; set; }
        public Nullable<System.DateTime> canceldate { get; set; }
        public short paymentType { get; set; }
        public short isGuarantee { get; set; }
        public string orderbreakfast { get; set; }
        public string reserved_field { get; set; }
        public Nullable<short> isautoconfirm { get; set; }
        public Nullable<System.DateTime> payTime { get; set; }
        public Nullable<short> shuadan { get; set; }
        public Nullable<decimal> cancelfee { get; set; }

        public string CurrencyCode { get; set; }

        public decimal? MemberPrice { get; set; }

        public string BuyerNick { get; set; }

        public short hasChange { get; set; }
    }
}
