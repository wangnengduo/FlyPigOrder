using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Hotel.Channel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Repository.Channel
{
    public class BigTreeRepository
    {
        private SqlSugarClient sqlSugarClient;

        public BigTreeRepository(ShopType shop)
        {
            sqlSugarClient = ProductChannelFactory.CreateSqlSugar(shop);
        }


        /// <summary>
        /// 获取房态
        /// </summary>
        /// <param name="ratePlanId"></param>
        /// <param name="checkInTime"></param>
        /// <param name="checkOutTime"></param>
        /// <returns></returns>
        public List<BigTreeRoomStatusDto> GetRoomRateStatus(string ratePlanId, DateTime checkInTime, DateTime checkOutTime)
        {
            string sql = string.Format(@"SELECT  rr.roomstate Status,rp.status RoomStatus,rr.startDate StartDate,rr.taobaoprice SalePrice FROM dbo.roomRates rr with(nolock) LEFT JOIN dbo.ratePlans rp with(nolock) ON rp.ratePlanId = rr.ratePlanId
 WHERE rp.ratePlanId='{0}' AND rr.startDate>='{1}' AND rr.startDate<'{2}'", ratePlanId, checkInTime.ToString("yyyy-MM-dd"), checkOutTime.ToString("yyyy-MM-dd"));
            return sqlSugarClient.SqlQueryable<BigTreeRoomStatusDto>(sql).ToList();
        }


        /// <summary>
        /// 获取酒店价格
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="rpid"></param>
        /// <param name="checkInTime"></param>
        /// <param name="checkOutTime"></param>
        /// <returns></returns>
        public TmallHotelPriceInfo GetHotelPriceInfo(string rid, string rpid, DateTime checkInTime, DateTime checkOutTime)
        {
            string sql = string.Format(@"
SELECT  h.hotelName, hr.rid RoomId,hr.roomName RoomName,rp.ratePlanName RatePlanName,hr.roomState RoomStatus,rp.roomBreakfast Breakfast,rp.ratePlanId RatePlanId,rp.status RatePlanStatus,rr.startDate AS CheckInTime,rr.salePrice AS BasePrice,rr.taobaoprice AS SalePrice, rr.roomstate  as Available,rp.paymentType PaymentType
                FROM dbo.hotelRooms hr INNER JOIN dbo.ratePlans rp ON rp.rid = hr.rId
                LEFT JOIN dbo.roomRates rr with(nolock) ON rr.ratePlanId=rp.ratePlanId
                left join hotels h with(nolock) on h.hId=hr.hId
                WHERE hr.rId='{0}' AND rp.ratePlanId='{1}' AND rr.startDate>='{2}' AND rr.startDate<'{3}'
                ", rid, rpid, checkInTime.ToString("yyyy-MM-dd"), checkOutTime.ToString("yyyy-MM-dd"));

            TmallHotelPriceInfo order = new TmallHotelPriceInfo();
            var result = sqlSugarClient.SqlQueryable<DetailOrderDto>(sql).ToList();
            if (result.Count > 0)
            {
                var roomInfo = result.FirstOrDefault();
                order.RoomName = roomInfo.RoomName;
                order.RatePlanName = roomInfo.RatePlanName;
                order.PaymentType = roomInfo.PaymentType;
                order.HotelName = roomInfo.HotelName;
                if (roomInfo.RoomStatus == 0 || roomInfo.RatePlanStatus == 0)
                {
                    return order;
                }

                string priceStr = "";
                string datestr = "";
                int datePrice = 0;
                foreach (var item in result)
                {
                    if (item.Available == 0)
                        return order;

                    datestr = item.CheckInTime.ToString("yyyy-M-dd");
                    priceStr += "price" + datestr + "|" + item.BasePrice + "|money" + datestr + "|0|";
                    datePrice += item.BasePrice;
                }
                order.PriceStr = priceStr;
                order.DatePrice = datePrice;
            }
            return order;
        }


    }
}
