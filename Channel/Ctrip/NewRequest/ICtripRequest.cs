using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtripApi.NewRequest
{
    public interface ICtripRequest<ICtripResponse>
    {
        string ICODE { get; }
    }
}
