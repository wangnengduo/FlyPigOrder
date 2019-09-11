using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class HotelGoodsPriceRequest : IBaseRequest<HotelGoodsPriceResponse>
    {
        public string Method => "hotel.goods.price";

        public List<long> GoodsIds { get; set; }

        public List<long> HotelIds { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }


    }
}
