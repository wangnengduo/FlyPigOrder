using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.MeiTuan.Entity.mtPriceCacheEntity
{
    public class mtPriceCache
    {
        public string AddBed;
        public string AddValue;
        public string AdWay;
        public string Area;
        public string BedType;
        public string Desc;
        public string Detail;
        public int ExtraMoney;
        public bool IsHasMobile;
        public string Net;
        public string Note;
        public List<RatePlan> RatePlan;
        public string RoomFloor;
        public string RoomId;
        public string RoomName;
        public int RoomPrice;
        public string RoomTypeId;
        public string Window;
        public string XRoomId;
        public string XRoomName;
    }
    public class RatePlan
    {
        public string AddBed;
        public string AddValue;
        public bool Available;
        public string AveragePrice;
        public string BedType;
        public string Breakfast;
        public decimal CnPrice;
        public string Currency;
        public string Desc;
        public int ExtraBonus;
        public string Guarantee;
        public bool InstantCode;
        public string Net;
        public string PayMent;
        public string Promotion;
        public string RatePlanId;
        public string RatePlanName;
        public List<Rate> Rates;
        public decimal RMBPrice;
        public string RoomId;
        public string RoomName;
        public string RoomTypeId;
        public string RoomTypeImg;
        public decimal SMoney;
        public VouchResult Vouch;
        public string VouchNode;
        public string XRatePlanName;

        //public RatePlan();

        public string CurrencyCode { get; set; }
        public int CurrentAlloment { get; set; }
        public string CustomerType { get; set; }
        public string InvoiceMode { get; set; }
        public int MinAmount { get; set; }
        public int MinDays { get; set; }
        public int RoomBonus { get; set; }
        public string RoomInventoryStatusCode { get; }
    }
    public class Rate : CommonRate
    {
        public double AmountBeforeTax;
        public bool Available;
        public int Bonus;
        public decimal CostPrice;
        public bool InstantCode;

        //public Rate();

        public string DateStr { get; }
        public string WeekStr { get; }
    }
    public class VouchResult
    {
        public string CNDescription;
        public int Count;
        public string ENDescription;
        public string IsVouch;
        public DateTime LastCancelTime;
        public string LastTime;
        public int? OverTime;
        public int VouchMoneyType;

        //public VouchResult();
    }
}
