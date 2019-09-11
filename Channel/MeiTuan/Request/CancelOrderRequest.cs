using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class CancelOrderRequest : IBaseRequest<CancelOrderResponse>
    {
        public string distributorOrderId { get; set; }
        public long mtOrderId { get; set; }
        public string cancelReason { get; set; }
        public int cancelCheck { get; set; }

        public string Method => "hotel.order.cancel";
    }
}

