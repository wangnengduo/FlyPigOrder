using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flypig.Order.Application.Order.Entities.Dto
{
    public class NoticeSimpleOrderDto
    {
        public long Aid { get; set; }

        public decimal? AlipayPay { get; set; }

        public string Remark { get; set; }

        public int Status { get; set; }

        public int OrderType { get; set; }
 
        public string OrderTypeStr { get; set; }

        public string SourceOrderId { get; set; }

        public DateTime CheckIn { get; set; }
 
        public string LogisticsStatus { get; set; }

        public DateTime OrderTime { get; set; }
    }
}
