using DaDuShi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Response
{
    public class WebServiceResponse<T>
    {
        public APIErrCode ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public bool Successful { get; set; }

        public T ResponseEt { get; set; }

        public WebServiceResponse()
        {
            Successful = true;
        }
    }
}
