using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Domestic
{
    public class DomesticRoomRate
    {
        public string Hid { get; set; }

        public string Rid { get; set; }

        public string RpId { get; set; }

        public int? Inventory { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// 卖价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 底价
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// 原币种价格
        /// </summary>
        public decimal CurrencyPrice { get; set; }

        public string CurrencyCode { get; set; }

        public int Status { get; set; }
    }
}
