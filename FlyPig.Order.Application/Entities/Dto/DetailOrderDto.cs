using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Dto
{
    public class DetailOrderDto
    {
        public string HotelName { get; set; }
        public int RoomId { get; set; }

        public string RoomName { get; set; }

        public string RatePlanName { get; set; }

        public short RoomStatus { get; set; }

        public string Breakfast { get; set; }

        public int RatePlanId { get; set; }

        public short RatePlanStatus { get; set; }

        public DateTime CheckInTime { get; set; }

        public short BasePrice { get; set; }

        public short SalePrice { get; set; }

        public short Available { get; set; }

        public short PaymentType { get; set; }

        public short IsHourRoom { get; set; }
    }
}
