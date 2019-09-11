using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class HotelGoodsPriceResponse : BaseResponse
    {
        public HotelGoodsPriceResult Result { get; set; }
    }

    public class HotelGoodsPriceResult
    {
        public List<HotelGoodsPrice> goodsPrices { get; set; }
    }


    public class HotelGoodsPrice
    {
        public long hotelIds { get; set; }

        public long goodsId { get; set; }

        public List<PriceModelsItem> priceModels { get; set; }
    }



}
