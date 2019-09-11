using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Entities.Enum;
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

    }
}