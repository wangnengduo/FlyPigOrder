using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Notice
{
    public class OrderStatusChangeNotice
    {
        public string distributorOrderId { get; set; }
        public long mtOrderId { get; set; }
        public int orderStatus { get; set; }

        public string desc { get; set; }
    }
}
