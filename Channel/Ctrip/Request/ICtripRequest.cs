using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Request
{
    public interface IBaseRequest<ICtripResponse>
    {
        string Method { get;  }

        string Url { get;  }
    }
}
