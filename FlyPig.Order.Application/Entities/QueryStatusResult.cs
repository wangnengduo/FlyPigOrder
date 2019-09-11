using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    [XmlRoot("Result")]
    public class QueryStatusResult : TaoBaoResultXml
    {
        // ResultCode
        //   0 查询成功
        //-300 查询失败
        //-301 自定义原因 Message 填入原因
        /// <summary>
        /// 1，已确认 
        /// 4，已取消 
        /// 100 ， 不存在
        /// </summary>
        ///第三方系统订单状态ResultCode 为 0时，此字段为必填)
        [XmlElement]
        public string Status { get; set; }

        [XmlElement]
        public string OrderId { get; set; }

        /// <summary>
        /// PMS的确认单号
        /// </summary>
        [XmlElement]
        public string PmsResID { get; set; }

        //[XmlElement]
        //public string Comment { get; set; }
    }
}
