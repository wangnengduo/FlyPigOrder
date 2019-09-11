using Flypig.Order.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flypig.Order.Application.Order
{
    public interface IOrderChannel
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="id"></param>
        ServiceResult CreateOrder(int id);

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        ServiceResult CancelOrder(int id);
    }
}
