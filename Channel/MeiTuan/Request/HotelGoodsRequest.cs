using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class HotelGoodsRequest : IBaseRequest<HotelGoodsResponse>
    {
        public string Method => "hotel.goods.rp";

        public List<long> HotelIds { get; set; }

        public string CheckinDate { get; set; }

        public string CheckoutDate { get; set; }

        public int GoodsType { get; set; }

    }
}
