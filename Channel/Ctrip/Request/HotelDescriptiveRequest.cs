using Ctrip.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Request
{
    public class HotelDescriptiveRequest
    {
        [RequsetNode("OTA_HotelDescriptiveInfoRQ")]
        public OTA_HotelDescriptiveInfoRQ OTA_HotelDescriptiveInfo { get; set; }
        public HotelDescriptiveRequest() { this.OTA_HotelDescriptiveInfo = new OTA_HotelDescriptiveInfoRQ(); }
    }

    public class OTA_HotelDescriptiveInfoRQ
    {
        [RequsetNode("OTA_HotelDescriptiveInfoRQ", "Version", false)]
        public string Version { get; set; }
        [RequsetNode("OTA_HotelDescriptiveInfoRQ", "xsi:schemaLocation", false)]
        public string xsi_schemaLocation { get; set; }
        [RequsetNode("OTA_HotelDescriptiveInfoRQ", "xmlns", false)]
        public string xmlns { get; set; }
        [RequsetNode("OTA_HotelDescriptiveInfoRQ", "xmlns:xsi", false)]
        public string xmlns_xsi { get; set; }

        [RequsetNode("HotelDescriptiveInfos")]
        public HotelDescriptiveInfos HotelDescriptiveInfos { get; set; }

        public OTA_HotelDescriptiveInfoRQ()
        {
            this.Version = "1.0";
            this.HotelDescriptiveInfos = new HotelDescriptiveInfos();
            this.xsi_schemaLocation = "http://www.opentravel.org/OTA/2003/05 OTA_HotelDescriptiveInfoRQ.xsd";
            this.xmlns = "http://www.opentravel.org/OTA/2003/05";
            this.xmlns_xsi = "http://www.w3.org/2001/XMLSchema-instance";
        }
    }
    public class HotelDescriptiveInfos
    {
        [RequsetNode("HotelDescriptiveInfo")]
        public List<HotelDescriptiveInfo> HotelDescriptiveInfoList { get; set; }
        public HotelDescriptiveInfos()
        {
            this.HotelDescriptiveInfoList = new List<HotelDescriptiveInfo>();
        }
    }
    public class HotelDescriptiveInfo
    {
        [RequsetNode("HotelDescriptiveInfo", "HotelCode", false)]
        public string HotelCode { get; set; }
        [RequsetNode("HotelDescriptiveInfo", "PositionTypeCode", true)]
        public string PositionTypeCode { get; set; }
        /// <summary>
        /// 是否请求酒店类信息；bool类型，True表示需要发送酒店类信息；
        /// </summary>
        [RequsetNode("HotelInfo", "SendData", true)]
        public bool HotelInfoSendData { get; set; }
        /// <summary>
        /// 是否发送客房信息；bool类型，True表示需要发送
        /// </summary>
        [RequsetNode("FacilityInfo", "SendGuestRooms", true)]
        public bool SendGuestRooms { get; set; }
        /// <summary>
        /// 是否发送景点地标信息；bool类型，True表示需要发送；可空
        /// </summary>
        [RequsetNode("AreaInfo", "SendAttractions", true)]
        public bool SendAttractions { get; set; }
        /// <summary>
        /// 是否发送娱乐区域信息；bool类型，True表示需要发送；可空
        /// </summary>
        [RequsetNode("AreaInfo", "SendRecreations", true)]
        public bool SendRecreations { get; set; }
        /// <summary>
        /// 是否发送联系方式类数据，bool类型，True表示需要发送；可空
        /// </summary>
        [RequsetNode("ContactInfo", "SendData", true)]
        public bool ContactInfoSendData { get; set; }
        /// <summary>
        /// 是否发送多媒体数据，bool类型，True表示需要发送；可空
        /// </summary>
        [RequsetNode("MultimediaObjects", "SendData", true)]
        public bool MultimediaSendData { get; set; }

        public HotelDescriptiveInfo()
        {
            this.HotelInfoSendData = true;
            this.SendGuestRooms = true;
            this.SendAttractions = true;
            this.SendRecreations = true;
            this.ContactInfoSendData = true;
            this.PositionTypeCode = "501";
        }
    }
}
