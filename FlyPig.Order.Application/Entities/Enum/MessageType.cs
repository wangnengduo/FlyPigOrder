using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Enum
{
    public enum MessageType
    {
        [Description("满房")]
        Full = 1,

        [Description("沟通")]
        Communication = 2,


        [Description("开具发票")]
        Invoice = 3,

        [Description("提醒申退")]
        Remind = 4,

        [Description("查无")]
        CheckNo = 5,

        [Description("部分退款")]
        Refund = 6

    }
}
