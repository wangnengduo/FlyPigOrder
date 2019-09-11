using Ctrip.Common;
using Ctrip.Config;
using Ctrip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Request
{
    public class D_HotelOrderMiniInfoRequest : IBaseRequest<HotelOrderMiniInfoResponse>
    {

        public D_HotelOrderMiniInfoRequest(ApiConfig config)
        {
            this.HotelOrderMiniInfoRequest = new HotelOrderMiniInfoRequest(config);
        }

        [RequsetNode("HotelOrderMiniInfoRequest")]
        public HotelOrderMiniInfoRequest HotelOrderMiniInfoRequest { get; set; }
        public string Method => "D_HotelOrderMiniInfo";
        public string Url => "/hotel/D_HotelOrderMiniInfo.asmx";

    }
    public class HotelOrderMiniInfoRequest
    {
        [RequsetNode("AllianceId")]
        public string AllianceId { get; set; }
        [RequsetNode("Sid")]
        public string Sid { get; set; }
        /// <summary>
        /// 携程订单号
        /// </summary>
        [RequsetNode("OrderId")]
        public string OrderId { get; set; }
        public HotelOrderMiniInfoRequest(ApiConfig apiConfig)
        {
            this.AllianceId = apiConfig.AllianceID;
            this.Sid = apiConfig.SID;
        }
    }
}
