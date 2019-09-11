using FlyPig.Order.Application.MT.Entity;
using FlyPig.Order.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Repository.Channel
{
    public class MeiTuanRepository
    {
        public MeiTuanRoomInfo GetRoomInfo(string hid, string rpid)
        {
            string sql = string.Format(@"     SELECT p.pointName hotelname, rt.roomName,cast(rt.roomid as varchar(50))  rid, 
    (CASE WHEN invoiceMode=1 THEN goodsName+' [酒店开具发票]' ELSE goodsName end)   rateplanname FROM dbo.mt_goods rp(nolock) 
            LEFT JOIN dbo.mt_roominfo rt(nolock) ON rp.roomid=rt.roomId
            left join  mt_poi p on p.hotelId=rt.hotelId
            WHERE    rp.hotelid={1}  AND rp.goodsid={0}    ", rpid, hid);

            return SqlSugarContext.ResellbaseInstance.SqlQueryable<MeiTuanRoomInfo>(sql).First();
        }


        /// <summary>
        /// 获取价格计划发票
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public RatePlanInvoice GetRatePlanInvoice(string hotelId, string ratePlanId)
        {
            string sql = string.Format("  SELECT hotelid,roomid,goodsid,invoiceMode FROM dbo.mt_goods WHERE hotelid={0} and goodsid={1}", hotelId, ratePlanId);
            return SqlSugarContext.ResellbaseInstance.SqlQueryable<RatePlanInvoice>(sql).First();
        }

 

    }
}
