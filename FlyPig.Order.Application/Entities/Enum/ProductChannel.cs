using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Enum
{
    /// <summary>
    /// 产品源
    /// </summary>
    public enum ProductChannel
    {
        /// <summary>
        /// 自签
        /// </summary>
        [Description("2")]
        BigTree = 1,


        /// <summary>
        /// 去哪儿HDS
        /// </summary>
        [Description("4")]
        Hds = 2,

        /// <summary>
        /// 同程
        /// </summary>
        [Description("6")]
        LY = 3,

        /// <summary>
        /// 艺龙
        /// </summary>
        [Description("3")]
        Elong = 4,

        /// <summary>
        /// 美团
        /// </summary>
        [Description("9")]
        MT = 5,

        /// <summary>
        /// 捷旅
        /// </summary>
        [Description("10")]
        JL = 6,

        /// <summary>
        /// 喜玩
        /// </summary>
        [Description("1")]
        XW = 7,

        /// <summary>
        /// 携程
        /// </summary>
        [Description("5")]
        Ctrip = 8,


        /// <summary>
        /// 美团
        /// </summary>
        [Description("13")]
        MTNew = 9,

        /// <summary>
        /// 大都市
        /// </summary>
        [Description("14")]
        DDS = 10,

    }
}
