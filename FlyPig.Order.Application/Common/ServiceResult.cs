using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flypig.Order.Application.Common
{
    public class ServiceResult
    {
        public bool IsSucess { get; private set; }
        public bool IsError { get; private set; }

        public bool IsException { get; private set; }

        public string Message { get; private set; }

        public object Data { get; private set; }

        public void AppendMessage(string message, params object[] obj)
        {
            this.IsSucess = true;
            this.IsError = false;
            this.IsException = false;
            this.Message += string.Format("{0}\r\n", string.Format(message, obj));
        }

        public ServiceResult SetError(string message, params object[] obj)
        {
            string msg = string.Empty;
            if (obj.Length == 0)
            {
                msg = message;
            }
            else
            {
                msg = string.Format(message, obj);
            }
            this.IsSucess = false;
            this.IsError = true;
            this.IsException = false;
            this.Message = msg;
            return this;
        }

        public void SetData(object obj)
        {
            this.Data = obj;
        }

        public ServiceResult SetSucess(string message, params object[] obj)
        {
            string msg = string.Empty;
            if (obj.Length == 0)
            {
                msg = message;
            }
            else
            {
                msg = string.Format(message, obj);
            }
            this.IsSucess = true;
            this.IsError = false;
            this.IsException = false;
            this.Message = msg;
            return this;
        }
    }
}
