using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    /// <summary>
    /// 催促订单
    /// </summary>
    public class UrgeRQ : TaoBaoXml
    {
        [XmlIgnore]
        public override RequestXmlType XmlType
        {
            get { return RequestXmlType.UrgeRQ; }
        }

        [XmlElement]
        public long TaoBaoOrderId { get; set; }

        [XmlElement]
        public DateTime CheckIn { get; set; }

        [XmlElement]
        public DateTime CheckOut { get; set; }


        [XmlElement]
        public string Remark { get; set; }

        [XmlElement]
        public int UrgeType { get; set; }
    }
}
