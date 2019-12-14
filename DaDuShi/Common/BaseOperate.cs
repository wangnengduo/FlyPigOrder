using DaDuShi.Entities;
using DaDuShi.Request;
using DaDuShi.Response;
using DaDuShi.DaDSService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Common
{
    public abstract class BaseOperate
    {
        protected RequestData _Request;
        public BaseOperate()
        {
            _Request = new RequestData();
        }

        protected WebServiceResponse<T> HandleResponse<T>(Error ErrEt)
        {
            WebServiceResponse<T> re = new WebServiceResponse<T>();
            if (ErrEt != null)
            {
                re.Successful = false;

                try
                {
                    re.ErrCode = (APIErrCode)Convert.ToInt32(ErrEt.Code);
                    re.ErrMsg = ErrEt.Message;
                }
                catch (Exception)
                {
                    re.ErrCode = APIErrCode.API新错误代码;
                    re.ErrMsg = string.Format("ErrCode:{0}  Message:{1}", ErrEt.Code, ErrEt.Message);
                }
            }

            return re;
        }
    }
}
