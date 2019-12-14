using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Channel.DaDuShi.Order;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Hotel.Channel;
using FlyPig.Order.Application.Repository.Order;
using LingZhong.HotelManager.Application.Channel.Ctrip.Order;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlyPig.Order.Controllers
{
    public class FlyPigController : Controller
    {
        /// <summary>
        /// 店铺
        /// </summary>
        private ShopType Shop
        {
            get
            {
                var shop = Convert.ToInt32(ConfigurationManager.AppSettings["Shop"]);
                return (ShopType)shop;
            }
        }
        /// <summary>
        /// 订单处理(任你行店铺)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RenNiXing()
        {
            try
            {
                string xmlData = string.Empty;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Request.InputStream))
                {
                    xmlData = reader.ReadToEnd();
                }

                AliTripOrderService orderService = new AliTripOrderService(Shop);
                var result = orderService.ExcuteOrder(xmlData);
                return this.Xml(result);
            }
            catch (Exception ex)
            {
                return Content("");
            }
        }
        /// <summary>
        /// 获取携程状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrderStatus()
        {
            string result = string.Empty;
            try
            {
                string reqData = string.Empty;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(HttpContext.Request.InputStream))
                {
                    reqData = reader.ReadToEnd();
                }
                ChenYiCtripOrderChannel orderService = new ChenYiCtripOrderChannel(Shop);
                result = orderService.GetXcOrderStatus(reqData);
                return Content(result);
            }
            catch (Exception ex)
            {
                return Content("");
            }
        }

        /// <summary>
        /// 创建大都市订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string CreateOrderDDS(int aid)
        {
            string result = string.Empty;
            try
            {
                var orderChannel = ProductChannelFactory.GetOrderChannelByOrderType(17, Shop);
                ServiceResult serviceResult = orderChannel.CreateOrder(Convert.ToInt32(aid));
                result = serviceResult.Message;
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 取消大都市订单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string CancelOrderDDS(int aid)
        {
            string result = string.Empty;
            try
            {
                var orderChannel = ProductChannelFactory.GetOrderChannelByOrderType(17, Shop);
                ServiceResult serviceResult = orderChannel.CancelOrder(Convert.ToInt32(aid));
                result = serviceResult.Message;
                return result;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取大都市状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string OrderStatusDDS(string taobaoOrderId)
        {
            string result = string.Empty;
            var OrderStatusJson = new OrderStatusJson();
            try
            {
                ThirdOrderRepository orderChannel = new ThirdOrderRepository();
                result = orderChannel.GetOrderStatusDDS(taobaoOrderId);
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        /// <summary>
        /// 获取大都市状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getOrderStatusDDS(string taobaoOrderId)
        {
            string result = string.Empty;
            var OrderStatusJson = new OrderStatusJson();
            try
            {
                ThirdOrderRepository orderChannel =new ThirdOrderRepository();
                result = orderChannel.GetOrderStatusDDS(taobaoOrderId);
                OrderStatusJson.OrderStatus = result;
                return this.Jsonp(OrderStatusJson);
            }
            catch (Exception ex)
            {
                return this.Jsonp(OrderStatusJson);
            }
        }
        private class OrderStatusJson
        {
            public string OrderStatus { get; set; }
            public string RefundInfo { get; set; }
        }
    }
}