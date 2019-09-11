using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlyPig.Order.Controllers
{
    public class OrderController : Controller
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
        /// 直连订单处理入口 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Excute()
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
    }
}