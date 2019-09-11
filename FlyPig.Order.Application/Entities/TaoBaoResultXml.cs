using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    [XmlRoot("Result")]
    public class TaoBaoResultXml
    {
        [XmlElement]
        public string Message { get; set; }
        [XmlElement]
        public string ResultCode { get; set; }
    }

}
