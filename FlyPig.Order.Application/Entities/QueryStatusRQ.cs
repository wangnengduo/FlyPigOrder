using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    /// <summary>
    /// 查询订单状态xml
    /// </summary>
    [Serializable]
    public class QueryStatusRQ : TaoBaoXml
    {
        [XmlIgnore]
        public override RequestXmlType XmlType
        {
            get { return RequestXmlType.QueryStatusRQ; }
        }

        /// <summary>
        /// 第三方订单编号
        /// </summary>
        [XmlElement]
        public string OrderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement]
        public long TaoBaoOrderId { get; set; }

        [XmlElement]
        public string HotelId { get; set; }

    }
}
