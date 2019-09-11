using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class HotelIdListResponse : BaseResponse
    {

        /// <summary>
        /// 请求响应数据, 不同请求返回的结构不同
        /// </summary>
        public HotelIdListResult Result { get; set; }


    }

    public class HotelIdListResult
    {
        /// <summary>
        /// maxId标记值，用于下一页查询。当maxId为-1时，表示已经查询到最后一页
        /// </summary>
        public long MaxId { get; set; }

        /// <summary>
        /// 可分销的酒店ID列表
        /// </summary>
        public List<long> HotelIds { get; set; }
    }
}
