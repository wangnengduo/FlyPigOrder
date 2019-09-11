using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class HotelGoodsStatusRequest : IBaseRequest<HotelGoodsStatusResponse>
    {
        public string Method => "hotel.goods.status";

        public long hotelId { get; set; }

        public string checkinDate { get; set; }

        public string checkoutDate { get; set; }

        public int goodsType { get; set; }
    }
}
