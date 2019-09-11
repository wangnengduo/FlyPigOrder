using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Enum
{
    public enum OrderStatus
    {
        /// <summary>
        /// 取消订单
        /// </summary>
        [Description("已操作取消")]
        Cancel = 2,

        /// <summary>
        /// 已确认
        /// </summary>
        [Description("已确认发货")]
        Confirm = 6,

        /// <summary>
        /// 不确认
        /// </summary>
        [Description("已操作为不确认")]
        NotSure = 22


    }
}
