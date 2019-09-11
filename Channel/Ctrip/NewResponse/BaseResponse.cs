using Ctrip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtripApi.NewResponse
{
    public class BaseResponse : ICtripResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public ResponseStatus ResponseStatus { get; set; }


    }
}
