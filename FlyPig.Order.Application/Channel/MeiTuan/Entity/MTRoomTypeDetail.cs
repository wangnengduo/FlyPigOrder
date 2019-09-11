
using FlyPig.Order.Application.Entities.Domestic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Entity
{
    public class MTRoomTypeDetail
    {

        public string Id { get; set; }

        public string Hid { get; set; }

        public string Name { get; set; }

        public string Area { get; set; }

        public string Floor { get; set; }

        public short HasWindows { get; set; }

        public short WiFi { get; set; }

        public string BedType { get; set; }

        public string BedTypeSize { get; set; }

        List<DomesticRatePlan> RatePlanList { get; set; }
    }
}
