using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Framework.HttpWeb
{
    public class RequestModel
    {
        public RequestModel(HttpVerbs method)
            : this(method, "", "")
        {
        }

        public RequestModel(HttpVerbs method, string url, string cookie = "")
            : this(method, url, "", cookie)
        {
        }

        public RequestModel(HttpVerbs method, string url, string data, string cookie = "")
        {
            if (method == HttpVerbs.POST)
            {
                Method = HttpVerbs.POST;
                UserAgent = @"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; Zune 4.7; BOIE9;ZHCN)";
                Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, */*";
                ContentType = "application/x-www-form-urlencoded";
            }
            else
            {
                Method = HttpVerbs.GET;
                Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                UserAgent = "Mozilla/5.0 (SymbianOS/9.1; U; en-us) AppleWebKit/413 (KHTML, like Gecko) Safari/413";
                ContentType = "application/x-www-form-urlencoded";
                Data = data;
            }

            RequestHearder = new List<KeyValuePair<string, string>>();
            Cookie = cookie;
            Data = string.Empty;
            Encode = Encoding.UTF8;

        }

        public string Host { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 请求页面所获得的Cookie
        /// </summary>
        public string Cookie { get; set; }

        public CookieContainer Cookies { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public HttpVerbs Method { get; set; }

        /// <summary>
        /// Referer响应头
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// 文本类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// UserAgent头
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Accept头
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 代理IP
        /// </summary>
        public WebProxy ProxyObject { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        public List<KeyValuePair<string, string>> RequestHearder { get; set; }

        /// <summary>
        /// 编码方式
        /// </summary>
        public Encoding Encode { get; set; }

        public int TimeOut { get; set; }

    }
}
