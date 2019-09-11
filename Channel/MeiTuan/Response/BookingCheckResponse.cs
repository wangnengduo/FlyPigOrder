using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class BookingCheckResponse : BaseResponse
    {
        public BookingCheckResult Result { get; set; }

    }
    public class BookingCheckResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 校验成功
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PriceModelsItem> priceModels { get; set; }

        public int remainRoomNum { get; set; }
    }
 
}
