using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Entities
{
    public enum OperationCode
    {
        Json获取数据 = 101,
        CSV获取数据 = 201,
        依靠DB获取数据 = 301,
        XML获取数据 = 401,
        WebService获取数据 = 501,
        手动更新数据 = 601,

        获取酒店价格计划数据 = 2001,

        获取价格确认数据 = 2101,
        创建订单 = 2102,
        订单查询 = 2103,
        订单取消 = 2104,
        订单取消确认 = 2105,
        获取酒店信息 = 2106,
        获取酒店变化ID集合 = 2107,
        订单确认操作 = 2108,

        更新房型作业 = 3101,
        更新静态信息作业 = 3102,
        执行订单确认作业 = 3103,
        执行向喜玩后台推送订单数据作业 = 3104,

        异常捕捉 = 9999
    };
}
