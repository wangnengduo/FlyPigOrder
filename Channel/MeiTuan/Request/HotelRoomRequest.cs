using MeiTuan.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Request
{
    public class HotelRoomRequest : IBaseRequest<HotelRoomResponse>
    {
        public string Method => "hotel.realroom.info";

        public List<long> hotelIds { get; set; }

    }









}
