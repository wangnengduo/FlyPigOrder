using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Repository
{
    public class HotelRepository
    {

        public HotelRepository(ProductChannel channel)
        {
            Channel = channel;
        }

        public ProductChannel Channel { get; set; }

        /// <summary>
        /// 获取酒店附加标识信息
        /// </summary>
        /// <returns></returns>
        public AliTripHotelExtension GetHotelExtension(string hotelid, int source)
        {
            return SqlSugarContext.LingZhongInstance.Queryable<AliTripHotelExtension>().Where(u => u.HotelId == hotelid && u.Source == source).First();
        }
        public CtripHotel GetCtripHotel(string hotelid)
        {
            string sql = string.Format("select top 1 CtripStarRate from CtripHotel where hotelId = {0}", hotelid);
            return SqlSugarContext.CtripHotelInstance.SqlQueryable<CtripHotel>(sql).First();
        }
    }
}
