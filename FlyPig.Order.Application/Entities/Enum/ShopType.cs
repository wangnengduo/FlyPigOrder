using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Enum
{
    public enum ShopType
    {
        /// <summary>
        /// 凌众
        /// </summary>
        [DescriptionAttribute("凌众")]
        LingZhong = 5,

        /// <summary>
        /// 印迹
        /// </summary>
        [DescriptionAttribute("印迹")]
        YinJi = 6,

        /// <summary>
        /// 盛旅
        /// </summary>
        [DescriptionAttribute("盛旅")]
        ShengLv = 7,


        /// <summary>
        /// 任行
        /// </summary>
        [DescriptionAttribute("任行")]
        RenXing = 8,


        /// <summary>
        /// 任你行
        /// </summary>
        [DescriptionAttribute("任你行")]
        RenNiXing = 9,


        /// <summary>
        /// 印迹国旅
        /// </summary>
        [DescriptionAttribute("印迹国旅")]
        YinJiGuoLv = 10
    }
}
