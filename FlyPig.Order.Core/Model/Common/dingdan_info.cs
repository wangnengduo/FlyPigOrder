using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Core.Model.Common
{
    public class dingdan_info
    {
        public int id { get; set; }
        public string typeid { get; set; }
        public Nullable<int> userid { get; set; }
        public string username { get; set; }
        public string dingdan_num { get; set; }
        public Nullable<int> hotel_id { get; set; }
        public string hotel_name { get; set; }
        public Nullable<int> region_id { get; set; }
        public Nullable<int> city_id { get; set; }
        public Nullable<int> room_id { get; set; }
        public string room_name { get; set; }
        public int num { get; set; }
        public Nullable<System.DateTime> begin_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public int state { get; set; }
        public System.DateTime dingdan_date { get; set; }
        public string tel { get; set; }
        public Nullable<int> total { get; set; }
        public Nullable<int> isview { get; set; }
        public Nullable<System.DateTime> cstime { get; set; }
        public Nullable<int> d_check { get; set; }
        public string user_id { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string personnum { get; set; }
        public string arrivedate { get; set; }
        public string lastarrivedate { get; set; }
        public string shenfen { get; set; }
        public string personname { get; set; }
        public string personsex { get; set; }
        public string recheck { get; set; }
        public string mobile { get; set; }
        public string Qq { get; set; }
        public string snote { get; set; }
        public string roomno { get; set; }
        public string roomtype { get; set; }
        public Nullable<int> room_id2 { get; set; }
        public string roomcount { get; set; }
        public Nullable<System.DateTime> intime { get; set; }
        public Nullable<System.DateTime> outtime { get; set; }
        public Nullable<System.DateTime> lasttime { get; set; }
        public string roomnight { get; set; }
        public Nullable<int> totalmoney { get; set; }
        public string success { get; set; }
        public string cause { get; set; }
        public string caozuo { get; set; }
        public string beizhu { get; set; }
        public bool ishotelComment { get; set; }
        public bool isserviceComment { get; set; }
        public Nullable<int> total_r_money { get; set; }
        public string price_str { get; set; }
        public string true_price_str { get; set; }
        public Nullable<int> dianping_yet { get; set; }
        public Nullable<int> fuwudianping_yet { get; set; }
        public string duanxin { get; set; }
        public string pingtai { get; set; }
        public Nullable<int> tui_hotelid { get; set; }
        public Nullable<int> laiyuan_userID { get; set; }
        public Nullable<int> hotel_way { get; set; }
        public string ly_pingtai { get; set; }
        public Nullable<int> dingdan_way { get; set; }
        public string HotelID { get; set; }
        public string RatePlanID { get; set; }
        public string RoomTypeID { get; set; }
        public string HotelOrderWay { get; set; }
        public string CustomerOrderWay { get; set; }
        public string ConfirmRatePlanID { get; set; }
        public string ConfirmRoomTypeID { get; set; }
        public string ConfirmHotelID { get; set; }
        public Nullable<int> ExtraMoney { get; set; }
        public Nullable<int> ConfirmBonus { get; set; }
        public Nullable<int> Version { get; set; }
        public string SpecialMemo { get; set; }
        public Nullable<bool> PayBonus { get; set; }
        public Nullable<bool> isUsedCoupon { get; set; }
        public string CouponID { get; set; }
        public Nullable<bool> FaxReceived { get; set; }
        public string PayMent { get; set; }
        public string ElongOrderID { get; set; }
        public string ElongBookingUrl { get; set; }
        public Nullable<int> OrderBonus { get; set; }
        public Nullable<int> OrderTotalAmount { get; set; }
        public Nullable<int> ConfirmTotalAmount { get; set; }
        public Nullable<System.Guid> UserUnique { get; set; }
        public Nullable<bool> IsPayBonus { get; set; }
        public Nullable<int> PayBonusType { get; set; }
        public Nullable<bool> HaveBonus { get; set; }
        public Nullable<int> PriceOrderWay { get; set; }
        public string TradeNo { get; set; }
        public Nullable<int> TradeStatus { get; set; }
        public Nullable<bool> PayStatus { get; set; }
        public string PayMessage { get; set; }
        public Nullable<int> Bonus { get; set; }
        public string MemoForUser { get; set; }
        public Nullable<int> childrenuserid { get; set; }
        public Nullable<System.DateTime> outTimes { get; set; }
        public Nullable<int> peifu { get; set; }
        public Nullable<decimal> Refund { get; set; }
    }
}
