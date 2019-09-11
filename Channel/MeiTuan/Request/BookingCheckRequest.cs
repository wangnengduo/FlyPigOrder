using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class BookingCheckRequest : IBaseRequest<BookingCheckResponse>
    {
        public string Method => "hotel.order.check";

        public string HotelId { get; set; }

        public string GoodsId { get; set; }

        public string checkinDate { get; set; }

        public string checkoutDate { get; set; }

        public int RoomNum { get; set; }

        public int totalPrice { get; set; }
    }
}
