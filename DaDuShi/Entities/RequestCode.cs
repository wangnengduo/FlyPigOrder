using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Entities
{
    //调用api返回转换后的代码
    public enum RequestCode
    {
        API_用户认证失败 = 401,
        API_内部程序出错 = 402,
        API_订单号已存在 = 403,
        API_价格不正确 = 404,
        API_新错误代码 = 405,

        程序代码出错 = 501,

        成功 = 200,
        无数据返回 = 201
    }
}
