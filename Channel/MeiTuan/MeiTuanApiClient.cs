using MeiTuan.Common;
using MeiTuan.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MeiTuan.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using FlyPig.Order.Framework.Logging;
using FlyPig.Order.Framework.HttpWeb;

namespace MeiTuan
{
    public class MeiTuanApiClient
    {
        private readonly LogWriter logWriter;

        public MeiTuanApiClient() : this(MeiTuanConfigManager.ZhiHe)
        {
        }

        public MeiTuanApiClient(MeiTuanConfig config)
        {
            this.Config = config;
            logWriter = new LogWriter("meituan/request");
        }

        public string Url
        { 
            get
            {
                return "https://fenxiao.meituan.com/opdtor/api";
            }
        }


        /// <summary>
        /// 分销平台API版本号，目前版本号为1.0
        /// </summary>
        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        /// <summary>
        /// 10位时间戳。若请求发起时间与平台服务端接受请求的时间相差过大，平台将直接拒绝本次请求
        /// </summary>
        public long Timestamp
        {
            get
            {
                return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            }
        }


        public Random random = new Random();

        /// <summary>
        /// 随机32位正整数。与timestamp联合使用以防止重放攻击
        /// </summary>
        public long Nonce
        {
            get
            {
                return random.Next();
            }
        }

        /// <summary>
        /// 美团密钥配置
        /// </summary>
        public MeiTuanConfig Config { get; set; }

        public TResponse Excute<TResponse>(IBaseRequest<TResponse> request) where TResponse : IBaseResponse, new()
        {
            var sortedMap = new SortedDictionary<string, object>(new SortLowerComparer());
            sortedMap.Add("version", Version);
            sortedMap.Add("method", request.Method);
            sortedMap.Add("timestamp", Timestamp);
            sortedMap.Add("nonce", Nonce);
            sortedMap.Add("partnerId", Config.PartnerId);
            sortedMap.Add("accesskey", Config.Accesskey);
            string data = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            sortedMap.Add("data", data);
            string signature = GetSignature(sortedMap);
            sortedMap.Add("signature", signature);

            string json = JsonConvert.SerializeObject(sortedMap);

            var httpRequestModel = new RequestModel(HttpVerbs.POST);
            httpRequestModel.Url = Url;
            httpRequestModel.Data = json;
            httpRequestModel.ContentType = "application/json; charset=utf-8";
            httpRequestModel.Accept = "application/json";
            httpRequestModel.Encode = Encoding.UTF8;


            try
            {
                var content = HttpUtility.PostHtml(httpRequestModel);
                if (request.Method == "hotel.order.booking" || request.Method == "hotel.order.cancel")
                {
                    logWriter.Write("请求参数：{0}  \n响应参数：{1}", json, content);
                }
                var response = JsonConvert.DeserializeObject<TResponse>(content);
                return response;
            }
            catch (Exception ex)
            {
                logWriter.Write("请求参数：{ 0}  \n异常信息：{ 1}", json, ex.ToString());
                throw ex;
            }
        }

        private string GetSignature(SortedDictionary<string, object> sortedMap)
        {
            List<string> paramlist = sortedMap.Select(item => string.Format("{0}={1}", item.Key, item.Value.ToString())).ToList();
            String rawSignStr = string.Join("&", paramlist);
            Encoding encode = Encoding.UTF8;
            byte[] byteData = encode.GetBytes(rawSignStr);
            byte[] byteKey = encode.GetBytes(Config.Secretkey);
            HMACSHA1 hmac = new HMACSHA1(byteKey);
            CryptoStream cs = new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write);
            cs.Write(byteData, 0, byteData.Length);
            cs.Close();
            string signature = Convert.ToBase64String(hmac.Hash);
            return signature;
        }
    }



}
