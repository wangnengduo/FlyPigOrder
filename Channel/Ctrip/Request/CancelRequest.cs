
using Ctrip.Common;
using Ctrip.Config;
using Ctrip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctrip.Request
{
    public class CancelRequest : IBaseRequest<CancelResponse>
    {
        [RequsetNode("OTA_CancelRQ")]
        public OTA_Cancel OTA_Cancel { get; set; }

        public string Method => "OTA_Cancel";

        public string Url => "/Hotel/OTA_Cancel.asmx";

        public CancelRequest(ApiConfig config) { this.OTA_Cancel = new OTA_Cancel(config); }
    }
    public class OTA_Cancel
    {
        [RequsetNode("OTA_CancelRQ", "TimeStamp", false)]
        public string TimeStamp { get; set; }
        [RequsetNode("OTA_CancelRQ", "Version", false)]
        public string Version { get; set; }
        /// <summary>
        /// Type属性：ID类型，参考CodeList(UIT) 501 Ctrip订单号；28联盟的ID；503联盟的站点ID；1表示联盟用户在携程的uniqueid
        /// </summary>
        [RequsetNode("UniqueID")]
        public List<UniqueID> UniqueIDList { get; set; }
        [RequsetNode("Reasons")]
        public Reasons Reasons { get; set; }
        public OTA_Cancel(ApiConfig config)
        {
            this.TimeStamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff+08:00");
            this.Version = "1.0";
            this.UniqueIDList = new List<UniqueID>()
            {
                new UniqueID(){ Type="28",ID= config.AllianceID},
                new UniqueID(){Type="503",ID=config.SID},
                new UniqueID(){ Type="1",ID=config.UID}
            };
            this.Reasons = new Reasons();
        }
    }
    public class Reasons
    {
        /// <summary>
        /// 取消原因列表，（最多99个）
        /// </summary>
        [RequsetNode("Reason")]
        public List<Reason> ReasonList { get; set; }

        public Reasons() { this.ReasonList = new List<Reason>() { new Reason() { Type = "501" } }; }
    }
    public class Reason
    {
        [RequsetNode("Reason", "Type", false)]
        public string Type { get; set; }
        //501	行程改变                //507	入住登记不详细
        //502	无法满足需求            //508	其他
        //503	酒店价格倒挂            //509	客户通知取消：其它
        //504	其它途径预订            //510	内部原因
        //505	价格不准确              //511	酒店客房已满
        //506	重复预订                //512	酒店客房已满
    }
}
