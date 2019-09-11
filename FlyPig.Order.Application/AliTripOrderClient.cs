
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Common.Tmall;
using FlyPig.Order.Core;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api.Request;
using Top.Api.Response;

namespace Flypig.Order.Application.Order
{
    public class AliTripOrderClient
    {
        public ShopType Shop { get; set; }
        public TmallApiClient tmallApiClient;
        private SqlSugarClient sugarClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shop"></param>
        public AliTripOrderClient(ShopType shop)
        {
            this.Shop = shop;
            tmallApiClient = new TmallApiClient(shop);
            if (Shop == ShopType.ShengLv || Shop == ShopType.RenNiXing)
            {
                sugarClient = SqlSugarContext.ResellbaseInstance;
            }
            else
            {
                sugarClient = SqlSugarContext.BigTreeInstance;
            }
        }

        #region 通过淘宝单号获取订单信息
        /// <summary>
        /// 通过淘宝单号获取订单信息
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        public XhotelOrderSearchResponse GetTaoBaoOrderInfo(long taobaoOrderId)
        {

            var currentDate = DateTime.Now;
            var request = new XhotelOrderSearchRequest
            {
                CreatedStart = currentDate.AddDays(-30),
                CreatedEnd = currentDate,
                OrderTids = taobaoOrderId.ToString()
            };

            var response = tmallApiClient.Execute(request);
            return response;
        }
        #endregion

        #region 获取订单信息
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="orderTime"></param>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        public Top.Api.Domain.XHotelOrder GetOrderStatus(long taobaoOrderId)
        {
            try
            {
                DateTime orderTime = DateTime.Now.Date;
                var req = new XhotelOrderSearchRequest();
                req.OrderTids = taobaoOrderId.ToString();
                req.CreatedStart = orderTime.AddDays(-10);
                req.CreatedEnd = orderTime.AddDays(1);
                var rsp = tmallApiClient.Execute(req);

                if (rsp.HotelOrders.Count > 0)
                {
                    var hotelResult = rsp.HotelOrders.FirstOrDefault();
                    return hotelResult;
                }
            }
            catch
            {
            }
            return null;
        }
        #endregion

    }
}
