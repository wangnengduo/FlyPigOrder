using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    [XmlRoot("Result")]
    public class ValidateRQResult : TaoBaoResultXml
    {
        [XmlElement]
        public string InventoryPrice { get; set; }
    }
}
