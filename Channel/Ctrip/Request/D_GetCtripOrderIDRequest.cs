using Ctrip.Common;
using Ctrip.Config;
using Ctrip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Request
{
    public class D_GetCtripOrderIDRequest : IBaseRequest< D_GetCtripOrderIDResponse>
    {
        [RequsetNode("DomesticGetCtripOrderIDRequest")]
        public DomesticGetCtripOrderIDRequest DomesticGetCtripOrderIDRequest { get; set; }

        public string Method => "D_HotelOrderMiniInfo";

        public string Url => "/hotel/D_GetCtripOrderID.asmx";

        public D_GetCtripOrderIDRequest()
        {
        }

 
    }
    public class DomesticGetCtripOrderIDRequest
    {
        [RequsetNode("DistributorOrderIDList")]
        public DistributorOrderIDList DistributorOrderIDList { get; set; }
        public DomesticGetCtripOrderIDRequest() { this.DistributorOrderIDList = new DistributorOrderIDList(); }
    }
    public class DistributorOrderIDList
    {
        [RequsetNode("DistributorOrderIDDetail")]
        public List<DistributorOrderIDDetail> DistributorOrderIDDetails { get; set; }
        public DistributorOrderIDList() { this.DistributorOrderIDDetails = new List<DistributorOrderIDDetail>(); }
    }
    public class DistributorOrderIDDetail
    {
        [RequsetNode("DistributorOrderID")]
        public string DistributorOrderID { get; set; }
    }
}
