using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Dto
{
    public class BookingCheckInputDto
    {
        public string HotelId { get; set; }

        public string RoomTypeId { get; set; }

        public string RatePlanId { get; set; }

        public string RatePlanCode { get; set; }

        public long ? Rpid { get; set; }
        public long Hid { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public int RoomNum { get; set; }

        public int CustomerNumber { get; set; }

        public bool IsVirtual { get; set; }
        /// <summary>
        /// 正在传给淘宝的房型id
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 是否为飞猪程序自动跑（非真实客户请求）
        /// 1为是客户请求，0为程序请求
        /// </summary>
        public int IsCustomer { get; set; }

    }
}
