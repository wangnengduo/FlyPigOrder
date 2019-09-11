using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctrip
{
    public static class CustomerWrapper
    {
        public static IDictionary<string, string> dx = new Dictionary<string, string>();

        static CustomerWrapper()
        {
            if (dx.Count == 0)
            {
                dx.Add("7", "适用：中国大陆、中国香港、中国澳门、中国台湾客人专享");
                dx.Add("1", "适用：中国大陆客人专享");
                dx.Add("2", "适用：中国香港、中国澳门、中国台湾客人专享");
                //dx.Add("1", "适用：持中国身份证的居民");
            }
        }

        public static string GetDesc(string key)
        {
            string desc = string.Empty;
            if (dx.ContainsKey(key))
                desc = dx[key];
            return desc;
        }
    }

    [Serializable]
    public class TAuthorize
    {
        public string AID { get; set; }
        public string SID { get; set; }
        public string Access_Token { get; set; }
        public string Expires_In { get; set; }
        public string Refresh_Token { get; set; }
    }

    [Serializable]
    public class XZorder
    {
        public List<string> guestNames { get; set; }
        public int guestNum { get; set; }

        public decimal salePrice { get; set; }

        public decimal totalPrice { get; set; }
        public string specialRequest { get; set; }
        public DateTime comeDate { get; set; }
        public DateTime leaveDate { get; set; }
        public string lateArriveTime { get; set; }
        public string contactPhone { get; set; }
        public string contactName { get; set; }
        public string ratePlanCode { get; set; }
        public int roomNum { get; set; }
        public string hotelCode { get; set; }
        public string orderNo { get; set; }
        public string ratePlanCategory { get; set; }

        public string ratePlanID { get; set; }
        public string bookingCode { get; set; }
    }


    [Serializable]
    public class ZXcResponse
    {
        public string code { get; set; }
        public string version { get; set; }
        public ZXcResponseResult result { get; set; }
        public string body { get; set; }
        public string errMsg { get; set; }
    }

    [Serializable]
    public class ZXcResponseResult
    {
        public string orderId { get; set; }
        public string currencyCode { get; set; }
        public string status { get; set; }
        public decimal prepayPrice { get; set; }
        public string userOrderState { get; set; }
        public string orderStatus { get; set; }
    }

    [Serializable]
    public class ZXcResult
    {
        public bool Suc { get; set; }
        public string Message { get; set; }
    }
}
