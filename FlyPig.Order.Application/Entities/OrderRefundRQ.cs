using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    public class OrderRefundRQ : TaoBaoXml
    {
        [XmlIgnore]
        public override RequestXmlType XmlType
        {
            get { return RequestXmlType.OrderRefundRQ; }
        }

        [XmlElement]
        public long TaoBaoOrderId { get; set; }
        [XmlElement]
        public string OrderId { get; set; }
        /// <summary>
        /// 是否发货，true为已发货，false为未发货
        /// </summary>
        [XmlElement]
        public string Shipped { get; set; }
    }
}
