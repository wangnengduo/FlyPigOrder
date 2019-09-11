using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class OrderBookingResponse : BaseResponse
    {
        public OrderBookingResult Result { get; set; }
    }


    public class OrderBookingResult
    {
        public string distributorOrderId { get; set; }

        public long? mtOrderId { get; set; }

        public int code { get; set; }
        public string desc { get; set; }
    }
}
