using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    public class ValidateRQ : TaoBaoXml
    {
        [XmlIgnore]
        public override RequestXmlType XmlType
        {
            get { return RequestXmlType.ValidateRQ; }
        }


        [XmlElement]
        public long TaoBaoHotelId { get; set; }
        [XmlElement]
        public string HotelId { get; set; }
        [XmlElement]
        public long TaoBaoRoomTypeId { get; set; }
        [XmlElement]
        public string RoomTypeId { get; set; }
        [XmlElement]
        public long TaoBaoRatePlanId { get; set; }
        [XmlElement]
        public string RatePlanCode { get; set; }
        [XmlElement]
        public long TaoBaoGid { get; set; }
        [XmlElement]
        public DateTime CheckIn { get; set; }
        [XmlElement]
        public DateTime CheckOut { get; set; }
        [XmlElement]
        public int RoomNum { get; set; }
        [XmlElement]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// 1预付5面付
        /// </summary>
        [XmlElement]
        public int PaymentType { get; set; }
        [XmlElement]
        public string Extensions { get; set; }
    }
}
