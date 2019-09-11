using Ctrip.Common;
using Ctrip.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Request
{

    [Serializable]
    public class HeaderRequest
    {
        public ApiConfig Config
        {
            get; set;
        }

        [RequsetNode("Header", "AllianceID", false)]
        public string AllianceID { get; set; }
        [RequsetNode("Header", "SID", false)]
        public string SID { get; set; }
        [RequsetNode("Header", "TimeStamp", false)]
        public string TimeStamp { get; set; }
        [RequsetNode("Header", "RequestType", false)]
        public string RequestType { get; set; }
        [RequsetNode("Header", "Signature", false)]
        public string Signature
        {
            get
            {
                return GetSign();
            }
        }
        public HeaderRequest(ApiConfig config ,string method)
        {
            this.Config = config;
            this.SID = Config.SID;
            this.AllianceID = Config.AllianceID;

            this.TimeStamp = ApiConfigManager.GetTimeStamp().ToString();
            this.RequestType = method;
        }

        public string GetSign()
        {
            string s = string.Empty;
            string target = this.TimeStamp + Config.AllianceID + ApiConfigManager.GetMD5(Config.ApiKey) + Config.SID + this.RequestType;
            s = ApiConfigManager.GetMD5(target);
            return s;
        }

    }



}
