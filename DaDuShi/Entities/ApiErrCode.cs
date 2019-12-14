using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Entities
{
    public enum APIErrCode
    {
        用户认证失败 = 403,
        内部程序出错 = 500,
        订单号已存在 = -300,
        价格不正确 = -114,
        API新错误代码 = -99999
    }
}
