using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Dto
{
    public class BigTreeRoomStatusDto
    {
        public short RoomStatus { get; set; }

        public short Status { get; set; }

        public DateTime StartDate { get; set; }

        public short SalePrice { get; set; }

    }
}
