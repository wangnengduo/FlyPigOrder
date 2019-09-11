using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Common
{

    [Serializable]
    public class CtAuthorize
    {
        public string Access_Token { get; set; }
        public string AID { get; set; }
        public string SID { get; set; }
        /// <summary>
        /// Access Token剩余的有效时间，单位为秒。
        /// </summary>
        public int Expires_In { get; set; }
        public string Refresh_Token { get; set; }
        public DateTime ReqTime { get; set; }

        public bool IsExpire()
        {
            bool flag = false;
            if (Expires_In > 0)
            {
                if (DateTime.Now > ReqTime.AddSeconds(Expires_In))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }
    }
}
