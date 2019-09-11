using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Configuration;
using FlyPig.Order.Core;

namespace FlyPig.Order.Core
{
    /// <summary>
    /// 数据库操作上下文
    /// </summary>
    public class SqlSugarContext
    {

        #region 订单相关
        /// <summary>
        /// 凌众
        /// </summary>
        public static SqlSugarClient BigTreeInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("BigTree", SqlSugarConnection.BigTree);
            }
        }

        /// <summary>
        /// 盛旅
        /// </summary>
        public static SqlSugarClient ResellbaseInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("Resellbase", SqlSugarConnection.Resellbase);
            }
        }


        /// <summary>
        /// 辰亿
        /// </summary>
        public static SqlSugarClient TravelskyInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("Travelsky", SqlSugarConnection.Travelsky);
            }
        }
        #endregion

        #region 店铺相关
        /// <summary>
        /// 凌众
        /// </summary>
        public static SqlSugarClient LingZhongInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("LingZhong", SqlSugarConnection.LingZhong);
            }
        }

        /// <summary>
        /// 印迹
        /// </summary>
        public static SqlSugarClient YinJiInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("YinJi", SqlSugarConnection.YinJi);
            }
        }

        /// <summary>
        /// 盛旅
        /// </summary>
        public static SqlSugarClient ShengLvInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("ShengLv", SqlSugarConnection.ShengLv);
            }
        }

        /// <summary>
        /// 任你行
        /// </summary>
        public static SqlSugarClient RenNiXingInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("RenNiXing", SqlSugarConnection.RenNiXing);
            }
        }

        /// <summary>
        /// 任行
        /// </summary>
        public static SqlSugarClient RenXingInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("RenXing", SqlSugarConnection.RenXing);
            }
        }

        /// <summary>
        /// 印迹国旅
        /// </summary>
        public static SqlSugarClient GuoLvInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("GuoLv", SqlSugarConnection.GuoLv);
            }
        }
        #endregion


        /// <summary>
        /// 渠道
        /// </summary>
        public static SqlSugarClient ChannelInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("Channel", SqlSugarConnection.Channel);
            }
        }

        /// <summary>
        /// 99
        /// </summary>
        public static SqlSugarClient CtripHotelInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("CtripHotel", SqlSugarConnection.CtripHotel);
            }
        }
        /// <summary>
        /// 99
        /// </summary>
        public static SqlSugarClient HotelProductInstance
        {
            get
            {
                return SqlSugarFactory.GetSugarClient("HotelProduct", SqlSugarConnection.HotelProduct);
            }
        }
    }
}
