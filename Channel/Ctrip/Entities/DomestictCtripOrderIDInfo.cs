using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Entities
{
    [Serializable]
    public class DomestictCtripOrderIDInfo
    {
        /// <summary>
        /// 分销商单号，下单时候传入的单号
        /// </summary>
        public string DistributorOrderID { get; set; }
        public string CtriporderID { get; set; }
    }
}
