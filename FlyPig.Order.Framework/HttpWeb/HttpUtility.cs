
using FlyPig.Order.Framework.HttpWeb;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttpHelper = System.Web.HttpUtility;

namespace FlyPig.Order.Framework.HttpWeb
{
    public class HttpUtility
    {

        #region Get方式获取页面

        public static string GetHtml(string url)
        {
            return GetHtml(url, string.Empty);
        }

        public static string GetHtml(string url, string data)
        {
            return GetHtml(url, string.Empty, referer: null);
        }

        public static string GetHtml(string url, string data, string referer = "")
        {
            var requestModel = new RequestModel(HttpVerbs.GET) { Url = url, Data = data, Referer = referer };
            return GetHtml(requestModel);
        }

        /// <summary>
        /// Get方式获取页面
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="data">传送数据</param>
        /// <param name="cookie">回传Cookie</param>
        /// <param name="referer">Referer头</param>
        /// <returns></returns>
        public static string GetHtml(string url, string data, ref string cookie, string referer = "")
        {
            var requestModel = new RequestModel(HttpVerbs.GET) { Url = url, Data = data, Referer = referer };
            string content = GetHtml(requestModel);
            cookie = requestModel.Cookie;
            return content;
        }

        /// <summary>
        /// Get方式获取页面
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetHtml(RequestModel model)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(model.Url) as HttpWebRequest;
                request.MaximumAutomaticRedirections = 100;
                request.Referer = model.Referer;
                request.Timeout = 300000;
                request.AllowAutoRedirect = true;
                request.ContentType = model.ContentType;
                request.UserAgent = model.UserAgent;
                request.KeepAlive = true;
                if (model.Url.IndexOf("https") == 0)
                    ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
                if (model.RequestHearder != null)
                {
                    foreach (var item in model.RequestHearder)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }

                if (model.ProxyObject != null)
                {
                    request.Proxy = model.ProxyObject;
                }
                System.Net.ServicePointManager.DefaultConnectionLimit = 50;
                if (model.Host != null)
                    request.Host = model.Host;
                request.CookieContainer = GetCookieContainer("http://" + request.RequestUri.Authority, model.Cookie);
                request.Accept = model.Accept;
                request.Method = model.Method.ToString();
                request.ServicePoint.Expect100Continue = false;

                //  WriteToRequestStream(model.Data, request.GetRequestStream());
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                model.Cookie = GetCookieString(response.Cookies);
                var stream = response.GetResponseStream();
                string content = string.Empty;
                if (response.ContentEncoding.Contains("gzip"))
                {
                    using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(gzStream, model.Encode))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    using (DeflateStream dfs = new DeflateStream(stream, CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(dfs, model.Encode))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    content = StreamUtility.GetString(stream, model.Encode);
                }


                request.Abort();
                response.Close();
                return content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
 
        #endregion


        #region Post方式获取页面
        public static string PostHtml(string url, string data)
        {
            return PostHtml(url, data, string.Empty);
        }

        public static string PostHtml(string url, string data, string referer)
        {
            var requestModel = new RequestModel(HttpVerbs.POST) { Url = url, Data = data, Referer = referer };
            return PostHtml(requestModel);
        }

        /// <summary>
        /// Post方式获取页面
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="data">传递数据</param>
        /// <param name="cookie">回传cookie</param>
        /// <param name="referer">Referer头</param>
        /// <returns></returns>
        public static string PostHtml(string url, string data, ref string cookie, string referer = "")
        {
            var requestModel = new RequestModel(HttpVerbs.POST) { Url = url, Data = data, Referer = referer };
            string content = PostHtml(requestModel);
            cookie = requestModel.Cookie;
            return content;
        }

        /// <summary>
        /// Post方式获取页面
        /// </summary>
        /// <param name="model">请求模型</param>
        /// <returns></returns>
        public static string PostHtml(RequestModel model)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(model.Url) as HttpWebRequest;
                request.UserAgent = model.UserAgent;
                request.Referer = model.Url;
                request.ContentType = model.ContentType;

                request.Accept = model.Accept;
                request.KeepAlive = true;
                request.ServicePoint.Expect100Continue = false;
                request.CookieContainer = GetCookieContainer(request.Referer, model.Cookie);

                if (model.RequestHearder.Count > 0)
                {
                    foreach (var item in model.RequestHearder)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }

                if (model.ProxyObject != null)
                {
                    request.Proxy = model.ProxyObject;
                }

                if (model.TimeOut > 0)
                {
                    request.Timeout = model.TimeOut * 1000;
                    request.ReadWriteTimeout = model.TimeOut * 1000;
                }


                request.Method = model.Method.ToString();
                WriteToRequestStream(model.Data, request.GetRequestStream());
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                string content = string.Empty;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();
                    if (response.ContentEncoding.Contains("gzip"))
                    {
                        using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(gzStream, model.Encode))
                            {
                                content = reader.ReadToEnd();
                            }
                        }
                    }
                    else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    {
                        using (DeflateStream dfs = new DeflateStream(stream, CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(dfs, model.Encode))
                            {
                                content = reader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        content = StreamUtility.GetString(stream, model.Encode);
                    }

                    request.Abort();
                    response.Close();
                    model.Cookie = GetCookieString(response.Cookies);
                }

                return content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Post方式获取页面
        /// </summary>
        /// <param name="model">请求模型</param>
        /// <returns></returns>
        public static string PostHtmlCheck(RequestModel model)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(model.Url) as HttpWebRequest;
                request.UserAgent = model.UserAgent;
                request.Referer = model.Url;
                request.ContentType = model.ContentType;

                request.Accept = model.Accept;
                request.KeepAlive = true;
                request.ServicePoint.Expect100Continue = false;
                request.CookieContainer = GetCookieContainer(request.Referer, model.Cookie);

                if (model.RequestHearder.Count > 0)
                {
                    foreach (var item in model.RequestHearder)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }

                if (model.ProxyObject != null)
                {
                    request.Proxy = model.ProxyObject;
                }

                if (model.TimeOut > 0)
                {
                    request.Timeout = model.TimeOut * 1000;
                    request.ReadWriteTimeout = model.TimeOut * 1000;
                }
                else
                {
                    request.Timeout = 2000;
                    request.ReadWriteTimeout = 2000;
                }

                request.Method = model.Method.ToString();
                WriteToRequestStream(model.Data, request.GetRequestStream());
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                string content = string.Empty;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = response.GetResponseStream();
                    if (response.ContentEncoding.Contains("gzip"))
                    {
                        using (GZipStream gzStream = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(gzStream, model.Encode))
                            {
                                content = reader.ReadToEnd();
                            }
                        }
                    }
                    else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    {
                        using (DeflateStream dfs = new DeflateStream(stream, CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(dfs, model.Encode))
                            {
                                content = reader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        content = StreamUtility.GetString(stream, model.Encode);
                    }

                    request.Abort();
                    response.Close();
                    model.Cookie = GetCookieString(response.Cookies);
                }

                return content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public static string DownLoadHtml(string url, Encoding encoding)
        {
            string resuelt = string.Empty;
            try
            {
                byte[] buffer = null;
                using (WebClient client = new WebClient())
                {
                    buffer = client.DownloadData(url);
                }
                resuelt = encoding.GetString(buffer);
            }
            catch (Exception ex)
            {
            }
            return resuelt;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        public static void DownFile(string url, string filePath)
        {
            string parentPath = Directory.GetParent(filePath).FullName;
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }

            WebClient client = new WebClient();
            client.DownloadFile(url, filePath);
        }

        /// <summary>
        /// URL解码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string UrlDecode(string content)
        {
            return HttpHelper.UrlDecode(content);
        }

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string UrlEncode(string content)
        {
            return HttpHelper.UrlEncode(content);
        }

        #region 辅助方法
        /// <summary>
        /// 获得Cookie 容器
        /// </summary>
        /// <param name="referer"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        static CookieContainer GetCookieContainer(string referer, string cookie)
        {
            CookieContainer cc = new CookieContainer();
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                Uri site = new Uri(referer);
                string[] array = Regex.Split(cookie, ";");
                foreach (var item in array)
                {
                    cc.SetCookies(site, item.ToString());
                }
            }
            return cc;
        }

        /// <summary>
        /// 获得Cookie字符串
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        static string GetCookieString(CookieCollection cookies)
        {
            string cookie = string.Empty;
            foreach (Cookie item in cookies)
            {
                cookie += item.Name + "=" + item.Value + "; ";
            }
            return cookie;
        }

        /// <summary>
        /// 将内容写入Requst流
        /// </summary>
        /// <param name="data"></param>
        /// <param name="requestStream"></param>
        /// <returns></returns>
        static bool WriteToRequestStream(string data, Stream requestStream)
        {
            try
            {
                byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(data);
                var stream = requestStream;
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();
                return true;
            }
            catch(Exception ex)
            {
                throw new EndOfStreamException("写入请求流失败");
            }
        }
        #endregion

        /// <summary>  
        /// 获取真ip  
        /// </summary>  
        /// <returns></returns>  
        public static string GetRealIP()
        {
            string result = String.Empty;
            result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //可能有代理   
            if (!string.IsNullOrWhiteSpace(result))
            {
                //没有"." 肯定是非IP格式  
                if (result.IndexOf(".") == -1)
                {
                    result = null;
                }
                else
                {
                    //有","，估计多个代理。取第一个不是内网的IP。  
                    if (result.IndexOf(",") != -1)
                    {
                        result = result.Replace(" ", string.Empty).Replace("\"", string.Empty);

                        string[] temparyip = result.Split(",;".ToCharArray());

                        if (temparyip != null && temparyip.Length > 0)
                        {
                            for (int i = 0; i < temparyip.Length; i++)
                            {
                                //找到不是内网的地址  
                                if (IsIPAddress(temparyip[i]) && temparyip[i].Substring(0, 3) != "10." && temparyip[i].Substring(0, 7) != "192.168" && temparyip[i].Substring(0, 7) != "172.16.")
                                {
                                    return temparyip[i];
                                }
                            }
                        }
                    }
                    //代理即是IP格式  
                    else if (IsIPAddress(result))
                    {
                        return result;
                    }
                    //代理中的内容非IP  
                    else
                    {
                        result = null;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }
        public static bool IsIPAddress(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < 7 || str.Length > 15)
                return false;

            string regformat = @"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);

            return regex.IsMatch(str);
        }

    }

    internal class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public AcceptAllCertificatePolicy()
        {
        }
        public bool CheckValidationResult(ServicePoint sPoint,
           X509Certificate cert, WebRequest wRequest, int certProb)
        {
            // Always accept
            return true;
        }


    }
}
