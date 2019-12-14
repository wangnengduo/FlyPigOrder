using FlyPig.Order.Application.Channel.Ctrip;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Repository.Channel
{
    public class CtripRepository
    {
        /// <summary>
        /// 获取携程酒店信息
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public CtripHotel GetHotel(string hotelId)
        {
            //string sql = string.Format("select top 1 * from CtripHotel where HotelCode = {0}", hotelId);
            //return SqlSugarContext.LingZhongInstance.SqlQueryable<CtripHotel>(sql).First();
            string sql = string.Format("select top 1 hotelId as HotelCode,name as HotelName from CtripHotel where hotelId = {0}", hotelId);
            return SqlSugarContext.CtripHotelInstance.SqlQueryable<CtripHotel>(sql).First();
        }


        #region 获取房型信息
        /// <summary>
        /// 获取房型信息
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public CtripRoomType GetRoomType(string hotelId, string roomId)
        {
            //return SqlSugarContext.LingZhongInstance.Queryable<CtripRoomType>().Where(u => u.HotelId == hotelId && u.RoomId == roomId).First();

            var rt = new CtripRoomType();
            try
            {
                string sql =string.Format(@"SELECT roomJson as RoomName FROM dbo.ctripRoomCache WITH(NOLOCK) WHERE hotelId='{0}'", hotelId);
                var RoomNameobj = SqlSugarContext.CtripHotelInstance.SqlQueryable<CtripRoomType>(sql).First();
                if (RoomNameobj != null)
                {
                    if (!string.IsNullOrEmpty(RoomNameobj.RoomName))
                    {
                        var baseRts = Newtonsoft.Json.JsonConvert.DeserializeObject<TgCtripRooms>(RoomNameobj.RoomName);
                        var RoomTypeInfo = baseRts.RoomStaticInfos.Where(b => b.RoomTypeInfo.RoomTypeID == roomId).FirstOrDefault();
                        rt.RoomName = RoomTypeInfo.RoomTypeInfo.RoomTypeName;
                        rt.RoomId = roomId;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return rt;
        }


        #endregion
    }
}
