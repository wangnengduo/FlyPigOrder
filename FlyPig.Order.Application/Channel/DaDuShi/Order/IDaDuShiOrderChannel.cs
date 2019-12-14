using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.DaDuShi.Order
{
    public interface IDaDuShiOrderChannel : IOrderChannel
    {
        string RequestUrl { get; }

        ServiceResult SynChannelOrder(int aid);
    }
}
