using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Entity
{
    /// <summary>
    /// 价格计划发票信息
    /// </summary>
    public class RatePlanInvoice
    {
        public int HotelId { get; set; }


        public int RoomId { get; set; }

        public long GoodsId { get; set; }

        public short InvoiceMode { get; set; }

        public bool IsCommission { get; set; }
    }
}
