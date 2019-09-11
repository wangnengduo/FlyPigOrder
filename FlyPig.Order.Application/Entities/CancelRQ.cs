using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    public class CancelRQ : TaoBaoXml
    {
        [XmlIgnore]
        public override RequestXmlType XmlType
        {
            get { return RequestXmlType.CancelRQ; }
        }

        /// <summary>
        /// 第三方系统订单编号
        /// </summary>
        [XmlElement]
        public string OrderId { get; set; }
        [XmlElement]
        public string Reason { get; set; }
        [XmlElement]
        public long TaoBaoOrderId { get; set; }
        [XmlElement]
        public string HotelId { get; set; }

        [XmlElement]
        public string HardCancel { get; set; }

    }
}
