using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Config
{
    [Serializable]
    public class ApiConfig
    {
        /// <summary>
        /// 分销商ID
        /// </summary>
        public string AllianceID { get; set; }
        /// <summary>
        /// 站点ID
        /// </summary>
        public string SID { get; set; }
        public string ApiKey { get; set; }
        public string UID { get; set; }

        public string TokenKey { get; set; }

        public string Key { get; set; }
        public string Alias { get; set; }
    }
}
