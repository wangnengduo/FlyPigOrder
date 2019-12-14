using MeiTuan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Common
{
    public class DaDuShiConfigManager
    {
        public static DaDuShiConfig RenNiXing
        {
            get
            {
                return new DaDuShiConfig
                {
                    ClientID = "C668434",
                    LicenseKey = "Toptown#C668434",
                    Token = ""
                };
            }
        }
    }
}
