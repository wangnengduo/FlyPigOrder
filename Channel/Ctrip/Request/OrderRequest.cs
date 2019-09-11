
using Ctrip.Common;
using Ctrip.Config;
using Ctrip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctrip.Request
{
    public class OrderRequest : IBaseRequest<ResRetrieveResponse>
    {
        [RequsetNode("OTA_ReadRQ")]
        public OTA_ReadRQ OTA_ReadRQ { get; set; }

        public string Method => "OTA_Read";

        public string Url => "/Hotel/OTA_Read.asmx";

        public OrderRequest(ApiConfig config) { this.OTA_ReadRQ = new OTA_ReadRQ(config); }
    }
    public class OTA_ReadRQ
    {
        [RequsetNode("OTA_ReadRQ", "Version", false)]
        public string Version { get; set; }
        /// <summary>
        /// Type属性：ID类型，参考CodeList(UIT)，28联盟的ID；503联盟的站点ID；1表示联盟用户在携程的uniqueid，501 Ctrip订单号 ；string类型；必填
        /// </summary>
        [RequsetNode("UniqueID")]
        public List<UniqueID> UniqueIDList { get; set; }
        public OTA_ReadRQ(ApiConfig config)
        {
            this.Version = "2.0";
            this.UniqueIDList = new List<UniqueID>()
            {
                new UniqueID(){ Type="28",ID=config.AllianceID},
                new UniqueID(){Type="503",ID=config.SID},
                new UniqueID(){ Type="1",ID=config.UID}
            };
        }
    }
}
