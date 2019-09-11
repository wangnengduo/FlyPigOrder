using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class HotelGoodsStatusResponse : BaseResponse
    {
        public HotelGoodsStatusResult Result { get; set; }
    }

    public class HotelGoodsStatusResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int hotelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GoodsStatusesItem> goodsStatuses { get; set; }
    }

    public class GoodsStatusesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int goodsId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GoodsStatusesDayItem> goodsStatuses { get; set; }
    }

    public class GoodsStatusesDayItem
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
    }






}
