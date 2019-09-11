using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Entity
{
    public class MTReconciliationInfo
    {

        public DateTime Date { get; set; }

        public int DistributionId { get; set; }

        public string DistributionName { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int POI { get; set; }

        public long PlatformHotelId { get; set; }

        public string HotelName { get; set; }

        public string RoomType { get; set; }

        public string OrderNo { get; set; }

        public string OrderId { get; set; }

        public DateTime CheckInTime { get; set; }
        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }
        public int RoomNum { get; set; }
        public string RoomNight { get; set; }
        public string Occupancy { get; set; }
        public decimal Amount { get; set; }
        public decimal SettlementPrice { get; set; }

        public decimal Commission { get; set; }
        public string OrderStatus { get; set; }
        public string InvoiceMode { get; set; }

        public string SupplierOrderId { get; set; }

        public string OrderSource { get; set; }

        public string Remark { get; set; }

    }
}
