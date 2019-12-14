using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web.Mvc
{
    /// <summary>
    /// 扩展System.Mvc.Controller
    ///  </summary>
    public static class ControllerExtension
    {
        public static XmlResult Xml(this Controller request, object obj) { return Xml(obj, null, null, XmlRequestBehavior.DenyGet); }
        public static XmlResult Xml(this Controller request, object obj, XmlRequestBehavior behavior) { return Xml(obj, null, null, behavior); }
        public static XmlResult Xml(this Controller request, object obj, Encoding contentEncoding, XmlRequestBehavior behavior) { return Xml(obj, null, contentEncoding, behavior); }
        public static XmlResult Xml(this Controller request, object obj, string contentType, Encoding contentEncoding, XmlRequestBehavior behavior) { return Xml(obj, contentType, contentEncoding, behavior); }

        internal static XmlResult Xml(object data, string contentType, Encoding contentEncoding, XmlRequestBehavior behavior) { return new XmlResult() { ContentEncoding = contentEncoding, ContentType = contentType, Data = data, XmlRequestBehavior = behavior }; }

        public static JsonResult Jsonp(this Controller controller, object data)
        {
            FlyPig.Order.Common.Result.JsonpResult result = new FlyPig.Order.Common.Result.JsonpResult()
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return result;
        }
    }
}