using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Entities
{
    [Serializable]
    public class HeaderInfo
    {
        public bool ShouldRecordPerformanceTime { get; set; }
        public DateTime Timestamp { get; set; }
        public string ReferenceID { get; set; }
        public DateTime RecentlyTime { get; set; }
        public string AccessCount { get; set; }
        public string CurrentCount { get; set; }
        public DateTime ResetTime { get; set; }
        public string ResultCode { get; set; }

    }
}
