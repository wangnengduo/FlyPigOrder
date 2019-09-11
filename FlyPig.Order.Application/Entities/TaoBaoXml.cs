using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    public abstract class TaoBaoXml
    {
        [XmlElement]
        public AuthenticationToken AuthenticationToken { get; set; }

        [XmlIgnore]
        public abstract RequestXmlType XmlType { get; }
    }

    [Serializable]
    public enum RequestXmlType
    {
        BookRQ = 1,
        CancelRQ = 2,
        QueryStatusRQ = 3,
        PaySuccessRQ = 4,
        ValidateRQ = 5,
        OrderRefundRQ = 6,
        UrgeRQ = 7,
        Other = -1
    }
}
