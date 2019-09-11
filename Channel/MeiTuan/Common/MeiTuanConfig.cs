using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Common
{
    public class MeiTuanConfig
    {
        /// <summary>
        /// 平台分配给第三方渠道的分销业务ID
        /// </summary>
        public int PartnerId { get; set; }

        /// <summary>
        /// 平台分配给第三方渠道的安全凭证公钥
        /// </summary>
        public string Accesskey { get; set; }

        /// <summary>
        /// 用于验证此次请求合法性的签名（签名方法见下文）
        /// </summary>
        public string Secretkey { get; set; }
    }
}
