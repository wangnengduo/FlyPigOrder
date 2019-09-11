using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeiTuan.Common
{
    public class MeiTuanConfigManager
    {
 
        public static MeiTuanConfig ShengLv
        {
            get
            {
                return new MeiTuanConfig
                {
                    PartnerId = 984,
                    Accesskey = "00ed8bdcf4a7b82b7be7b03c13309079",
                    Secretkey = "5df6efbd3c67624de967feb707211ef7"
                };
            }
        }


        public static MeiTuanConfig ChenYi
        {
            get
            {
                return new MeiTuanConfig
                {
                    PartnerId = 434,
                    Accesskey = "844f3b6bf5f424eaa58d2907d4ceac30",
                    Secretkey = "5efa97c8b94e55e14dd228618b422127"
                };
            }
        }


        public static MeiTuanConfig XiWan
        {
            get
            {
                return new MeiTuanConfig
                {
                    PartnerId = 3221,
                    Accesskey = "3f61ab6e16fe798f0da0271fe4fc4372",
                    Secretkey = "c4c844f41bfd3920c4e9db89c3d654e2"
                };
            }
        }

        public static MeiTuanConfig ZhiHe
        {
            get
            {
                return new MeiTuanConfig
                {
                    PartnerId = 4903,
                    Accesskey = "9093e614e88c622a876693ba77d9c3e5",
                    Secretkey = "a8f819007526a448accecbed4f93e657"
                };
            }
        }

        public static MeiTuanConfig RenNiXing
        {
            get
            {
                return new MeiTuanConfig
                {
                    PartnerId = 4585,
                    Accesskey = "6a7db825f4d539b480c3551f02fe9a6c",
                    Secretkey = "0f85f04a5ef9d939acdd1975b4ae2e7b"
                };
            }
        }
    }
}
