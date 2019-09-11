using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    [XmlRoot("Result")]
    public class CancelRQResult : TaoBaoResultXml
    {
        //0 取消成功
        //-200 取消失败
        //-203 自定义原因  Message 填入原因

        /// <summary>
        /// 第三方系统订单号 第三方系统订单号
        /// </summary>
        [XmlElement]
        public string OrderId { get; set; }
    }
}
