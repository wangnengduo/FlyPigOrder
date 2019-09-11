using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip
{

    public class ResponseStatus
    {
        /// <summary>
        /// 
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Ack { get; set; }

        public string Build { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Errors { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Extension { get; set; }
    }
}
