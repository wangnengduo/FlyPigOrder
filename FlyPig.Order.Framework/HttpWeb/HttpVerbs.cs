using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Framework.HttpWeb
{
    /// <summary>
    /// 传送方式
    /// </summary>
    public enum HttpVerbs
    {
        /// <summary>
        /// 表单传送
        /// </summary>
        POST = 1,

        /// <summary>
        /// QueryString参数传送
        /// </summary>
        GET = 2
    }
}
