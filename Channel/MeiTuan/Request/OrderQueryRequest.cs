using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class OrderQueryRequest : IBaseRequest<OrderQueryResponse>
    {

        public List<OrderQueryItem> QueryParams { get; set; }

        public string Method => "hotel.order.query";
    }


    public class OrderQueryItem
    {

        public string DistributorOrderId { get; set; }

        public string MtOrderId { get; set; }
    }

}
