using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Order
{
    public interface IMeiTuanOrderChannel : IOrderChannel
    {

        string RequestUrl { get; }

        ServiceResult SynChannelOrder(int aid);

    }
}
