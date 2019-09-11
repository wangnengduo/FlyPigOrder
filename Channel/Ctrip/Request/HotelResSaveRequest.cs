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

    public class HotelResSaveRequest : IBaseRequest<HotelResSaveResponse>
    {
        [RequsetNode("OTA_HotelResSaveRQ")]
        public OTA_HotelResSaveRQ OTA_HotelResSaveRQ { get; set; }

        public string Method => "OTA_HotelRes";

        public string Url
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public HotelResSaveRequest(ApiConfig config)
        {
            this.OTA_HotelResSaveRQ = new OTA_HotelResSaveRQ(config);
        }

    }
    public class OTA_HotelResSaveRQ
    {
        [RequsetNode("OTA_HotelResSaveRQ", "TimeStamp", false)]
        public string TimeStamp { get; set; }
        [RequsetNode("OTA_HotelResSaveRQ", "Version", false)]
        public string Version { get; set; }
        [RequsetNode("UniqueID")]
        public List<UniqueID> UniqueIDList { get; set; }
        [RequsetNode("HotelReservations")]
        public HotelReservations HotelReservations { get; set; }


        public OTA_HotelResSaveRQ(ApiConfig config)
        {
            this.TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff+08:00");
            this.Version = "1.0";
            this.UniqueIDList = new List<UniqueID>()
            {
                new UniqueID(){ Type="504"},        //表示分销商订单号
                new UniqueID(){ Type="28",ID=config.AllianceID},
                new UniqueID(){Type="503",ID=config.SID},
                new UniqueID(){ Type="1",ID=config.UID}
            };
            this.HotelReservations = new HotelReservations();
        }
    }
}
