using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class HotelGoodsResponse : BaseResponse
    {
        public HotelGoodsResult Result { get; set; }
    }

    /// <summary>
    /// 酒店详情
    /// </summary>
    public class HotelGoodsResult
    {
        public List<HotelGoods> hotelGoods { get; set; }
    }

    public class HotelGoods
    {
        /// <summary>
        /// 
        /// </summary>
        public int hotelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GoodsItem> goods { get; set; }
    }



    public class BreakfastItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int breakfastType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int breakfastNum { get; set; }

        public int inStartDate { get; set; }

        public int inEndDate { get; set; }


    }

    public class RoomInfoListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int roomId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int hotelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string roomType { get; set; }
        /// <summary>
        /// 标准单人房
        /// </summary>
        public string roomName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string roomDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string useableArea { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string capacity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string window { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string windowView { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string windowBad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int extraBed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string internetWay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> roomBedInfos { get; set; }
    }

    public class CancelRulesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int cancelType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int aheadCancelDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int deductType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string aheadCancelHours { get; set; }
    }

    public class BookRulesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int serialCheckinMin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int serialCheckinMax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int roomCountMin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int roomCountMax { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int earliestBookingDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string earliestBookingHours { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int latestBookingDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string latestBookingHours { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int isDaybreakBooking { get; set; }
    }

    public class InvoiceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int invoiceMode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string supportTypes { get; set; }
    }

    public class PriceModelsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal salePrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal subPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double subRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int dayType { get; set; }
    }

    public class GoodsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int hotelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int goodsId { get; set; }
        /// <summary>
        /// 标准单人房-不含早-入住前一日12点前可取消
        /// </summary>
        public string goodsName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int goodsStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int goodsType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int confirmType { get; set; }


        public int thirdParty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int invRemain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int timeIntervalMin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int timeLimitMin { get; set; }


        public List<ReceptionTime> hourRoomReceptionTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public List<BreakfastItem> breakfast { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<RoomInfoListItem> roomInfoList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CancelRulesItem> cancelRules { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<BookRulesItem> bookRules { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public InvoiceInfo invoiceInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PriceModelsItem> priceModels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int needRealTel { get; set; }
    }

    public class ReceptionTime
    {

        public string startTime { get; set; }

        public string endTime { get; set; }
    }

}
