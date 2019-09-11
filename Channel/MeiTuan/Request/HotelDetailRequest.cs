using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class HotelDetailRequest : IBaseRequest<HotelDetailResponse>
    {
        public string Method => "hotel.detail";

        /// <summary>
        /// 需要查询酒店详情的酒店ID列表，一次最多查询20个
        /// </summary>
        public List<long> HotelIds { get; set; }

        /// <summary>
        /// 查询策略：
        ///1 基础信息
        ///2 扩展信息
        ///4 房型信息
        ///8 图片信息
        ///策略值可进行加和以查询多项信息 如：
        ///5 查询基础信息和图片信息
        ///15 查询全部信息
        /// </summary>
        public int Strategy { get; set; }


    }
}
