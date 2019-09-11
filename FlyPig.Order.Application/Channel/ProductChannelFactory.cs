
using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Channel.BigTree;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.MT.Order;
using FlyPig.Order.Application.MT.Order.Channel;
using FlyPig.Order.Application.Repository.Order;
using FlyPig.Order.Core;
using LingZhong.HotelManager.Application.Channel.Ctrip.Order;
using SqlSugar;
using System;

namespace FlyPig.Order.Application.Hotel.Channel
{
    /// <summary>
    /// 产品渠道工厂类
    /// </summary>
    public class ProductChannelFactory
    {
        /// <summary>
        /// 创建渠道
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="shop"></param>
        /// <returns></returns>
        public static IProductChannel CreateChannel(ProductChannel channel, ShopType shop)
        {
            switch (channel)
            {
                case ProductChannel.MT: return new MeiTuanProductChannel(shop, channel);
                //case ProductChannel.BigTree: return new BigTreeProductChannel(shop);
                case ProductChannel.Ctrip: return new CtripProductChannel(shop);
                default: throw new ArgumentNullException("channel");
            }
        }


        public static SqlSugarClient CreateSqlSugar(ShopType shop)
        {
            if (shop == ShopType.ShengLv)
                return SqlSugarContext.ShengLvInstance;
            else if (shop == ShopType.RenNiXing)
                return SqlSugarContext.RenNiXingInstance;
            else if (shop == ShopType.YinJi)
                return SqlSugarContext.YinJiInstance;
            else if (shop == ShopType.RenXing)
                return SqlSugarContext.RenXingInstance;
            else if (shop == ShopType.YinJiGuoLv)
                return SqlSugarContext.GuoLvInstance;
            else
                return SqlSugarContext.ShengLvInstance;
                //return SqlSugarContext.LingZhongInstance;
        }


        public static SqlSugarClient CreateOrderSqlSugar(ShopType shop)
        {
            if (shop == ShopType.ShengLv || shop == ShopType.RenNiXing)
                return SqlSugarContext.ResellbaseInstance;
            else
                return SqlSugarContext.ResellbaseInstance;
            //return SqlSugarContext.BigTreeInstance;
        }

 
        public static IOrderRepository CreateOrderRepository(ShopType shop, ProductChannel channel)
        {

            if (shop == ShopType.ShengLv || shop == ShopType.RenNiXing)
            {
                return new ShengLvOrderRepository();
            }
            else
            {
                //if (channel == ProductChannel.BigTree)
                //{
                //    return new BigTreeOrderRepository();
                //}
                //else
                //{
                    return new ThirdOrderRepository();
                //}
            }
        }

        public static IOrderChannel GetOrderChannelByOrderType(int orderType, ShopType shop)
        {
            switch (orderType)
            {
                //case 2:
                //    return new BigtreeOrderChannel();
                case 11:
                case 14:
                    return new ShengLvMeiTuanOrderChannel(shop);
                case 12:
                case 13:
                    return new ChenYiMeiTuanOrderChannel(shop);
                //case 3:
                //    return new TmallElongOrderChannel(shop);
                case 5:
                case 15:
                    return new CtripTmallOrderChannel(shop);
                case 16:
                    return new CtripTmallOrderChannel(shop);
                default:
                    throw new ArgumentNullException("orderType");
            }
        }

    }
}
