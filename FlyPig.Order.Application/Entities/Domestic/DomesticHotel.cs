using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Domestic
{
    public class DomesticHotel
    {
        public string HotelName { get; set; }

        public string Id { get; set; }


        public string District { get; set; }

        public string City { get; set; }

        public string CityName { get; set; }

        public string Privince { get; set; }

        public string PrivinceName { get; set; }

        public string Address { get; set; }

        public string TellPhone { get; set; }

        public string Lon { get; set; }

        public string Lat { get; set; }
    }
}
