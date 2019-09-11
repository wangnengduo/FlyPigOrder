using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Domestic
{
    public class DomesticRoomType
    {
        public string Id { get; set; }

        public string RoomTypeCode { get; set; }

        public string Hid { get; set; }

        public string Name { get; set; }

        public string Area { get; set; }

        public string Floor { get; set; }

        public short HasWindows { get; set; }

        public short WiFi { get; set; }

        public string BedType { get; set; }

        public string BedTypeSize { get; set; }

        public int? MaxOccupancy { get; set; }
    }
}
