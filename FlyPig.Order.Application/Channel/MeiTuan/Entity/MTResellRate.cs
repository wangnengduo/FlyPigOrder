using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Entity
{
    public class MTResellRate
    {
        public int HotelId { get; set; }

        public int RoomId { get; set; }

        public int GoodsId { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// 美团卖价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 美团房态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public decimal SubPrice { get; set; }

        /// <summary>
        /// 拥金率
        /// </summary>
        public double SubRatio    { get; set; }
    }
}
