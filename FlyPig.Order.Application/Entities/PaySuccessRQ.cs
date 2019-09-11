using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    public class PaySuccessRQ : TaoBaoXml
    {
        [XmlIgnore]
        public override RequestXmlType XmlType
        {
            get { return RequestXmlType.PaySuccessRQ; }
        }

        [XmlElement]
        public long TaoBaoOrderId { get; set; }

        [XmlElement]
        public string OrderId { get; set; }
        /// <summary>
        /// 支付宝交易号 28位
        /// </summary>
        [XmlElement]
        public string AlipayTradeNo { get; set; }
        /// <summary>
        /// 支付单位(分)
        /// </summary>
        [XmlElement]
        public long Payment { get; set; }
    }
}
