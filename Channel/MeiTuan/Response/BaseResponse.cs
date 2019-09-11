using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Response
{
    public class BaseResponse : IBaseResponse
    {
        /// <summary>
        /// 接口返回码（详见下文code映射表）    
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// code详细信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 分销业务ID
        /// </summary>
        public int? PartnerId { get; set; }


    }
}
