using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Config
{
    public class ApiConfigManager
    {
        public static string BaseUrl
        {
            get
            {
                string url = "http://openapi.ctrip.com";
                return url;
            }
        }


        public static string ServiceProxyUrl
        {
            get
            {
                string url = "https://sopenservice.ctrip.com/OpenService/ServiceProxy.ashx";
                return url;
            }
        }

        /// <summary>
        /// 辰亿
        /// </summary>
        public static ApiConfig ChenYi
        {

            get
            {
                return new ApiConfig
                {
                    AllianceID = "949869",
                    SID = "1529355",
                    ApiKey = "49983410f1b24b3c9eca0971f1e5bfac",
                    UID = "d30c37c4-bcf6-4a50-b711-170b3611e604",
                    TokenKey = "30329c8fbb5f41089b80c896dde6cf4c"
                };
            }
        }

        /// <summary>
        /// 致和
        /// </summary>
        public static ApiConfig ZhiHe
        {
            get
            {
                return new ApiConfig
                {
                    AllianceID = "949868",
                    SID = "1529353",
                    ApiKey = "fd2e1fb3681c43e4a4bcfe6b58222d28",
                    UID = "d8d3c544-6c72-4ec2-8fd6-3c7ab93082cd",
                    TokenKey = "67459c649822469684cba7d04c561866"
                };

            }
        }

        /// <summary>
        /// 岭南
        /// </summary>
        public static ApiConfig LingNan
        {
            get
            {
                return new ApiConfig
                {
                    AllianceID = "286728",
                    SID = "737005",
                    ApiKey = "1F11056C-33C4-482C-8196-2CB83F67ECC6",
                    UID = "d30c37c4-bcf6-4a50-b711-170b3611e604",
                    TokenKey = "a1fa2bd2daa64c18b8c593c657b0dd91"
                };

            }
        }

        /// <summary>
        /// 辰亿现付
        /// </summary>
        public static ApiConfig XianFu_Zhihe
        {
            get
            {
                return new ApiConfig
                {
                    AllianceID = "957911",
                    SID = "1541979",
                    ApiKey = "9adad23486b34df3afd453997a4bff55",
                    UID = "712d9dcf-d44b-4100-85a1-bc8baa8f8b82",
                    TokenKey = "8bde7f7d87a84ea1b5c3ce53055bd7f8"
                };
            }
        }

        /// <summary>
        /// 辰亿现付
        /// </summary>
        public static ApiConfig XianFu_ChenYi
        {
            get
            {
                return new ApiConfig
                {
                    AllianceID = "957909",
                    SID = "1541977",
                    ApiKey = "5f6fc05024ff4061b730e5c368e70d39",
                    UID = "f1c40b76-2309-4f64-976f-b902c0d3d6fe",
                    TokenKey = "78559c5250564908af2ee23f6ffe95fe"
                };
            }
        }

        /// <summary>
        /// 辰亿现付
        /// </summary>
        public static ApiConfig XianFu_LingNan
        {
            get
            {
                return new ApiConfig
                {
                    AllianceID = "957908",
                    SID = "1541967",
                    ApiKey = "4dd40373862043d1bebfdcc151b50e8f",
                    UID = "7e5d4721-c03a-4b80-91eb-c5e173dff84b",
                    TokenKey = "ad036bf5de114ddaaca1417fc4acde6e"
                };
            }
        }

        /// <summary>
        /// 致和促销
        /// </summary>
        public static ApiConfig ZhiHeC
        {
            get
            {
                return new ApiConfig
                {
                    Alias = "949868_1",
                    AllianceID = "949868",
                    SID = "2083340",
                    UID = "737afe15-40d1-4c57-8aed-02bc37b0e317",
                    Key = "2cf7b36aaff940789fbd34e41bc2a900",
                    TokenKey = "2cf7b36aaff940789fbd34e41bc2a900",
                    ApiKey = "83c4bf386bed42f2bd36a6ba5033a887"
                };

            }
        }


        public static string GetMD5(string str)
        {
            System.Security.Cryptography.MD5 m = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] s = m.ComputeHash(UnicodeEncoding.UTF8.GetBytes(str));
            return BitConverter.ToString(s).Replace("-", "").ToUpper();
        }

        public static DateTime ConvertDateTime(double tickCount)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);
            return dt1970.AddMilliseconds(tickCount).ToLocalTime();
        }

        public static long GetTimeStamp()
        {
            DateTime dt1970 = new DateTime(1970, 1, 1);
            return Convert.ToInt64((DateTime.UtcNow - dt1970).TotalSeconds);
        }
    }
}
