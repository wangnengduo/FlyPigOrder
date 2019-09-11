using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class HotelIdListRequest : IBaseRequest<HotelIdListResponse>
    {
        public string Method
        {
            get
            {
                return "hotel.poi.list";
            }
        }

        /// <summary>
        /// 查询上一页时返回的maxId标记值, 查询第一页时该值为0
        /// </summary>
        public long MaxId { get; set; }

        /// <summary>
        /// 每页酒店个数，取值范围[1,1000]
        /// </summary>
        public int pageSize { get; set; }

    }
}
