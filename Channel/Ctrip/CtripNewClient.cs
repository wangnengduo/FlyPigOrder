using Ctrip.Config;
using Ctrip.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlyPig.Order.Framework.HttpWeb;
using CtripApi.Common;
using FlyPig.Order.Framework.Logging;
using Ctrip.Common;
using CtripApi.NewRequest;
using CtripApi.NewResponse;

namespace Ctrip
{
    public class CtripNewClient
    {
        private readonly LogWriter noticeLogWriter;

        public static ApiConfig Config { get { return ApiConfigManager.ZhiHeC; } }

        public static CtAuthorize UpdateToken(string refreshToken)
        {
            string url = string.Format("http://openservice.Ctrip.com/openserviceauth/authorize.ashx?AID={0}&SID={1}&refresh_token={2}", Config.AllianceID, Config.SID, refreshToken);
            var content = HttpUtility.GetHtml(url);
            var result = JsonHelper.Deserialize<CtAuthorize>("json", content);
            return result;
        }

        public static CtAuthorize GetToken()
        {
            string url = string.Format("http://openservice.ctrip.com/openserviceauth/authorize.ashx?AID={0}&SID={1}&KEY={2}", Config.AllianceID, Config.SID, Config.TokenKey);
            var content = HttpUtility.GetHtml(url);
            var result = JsonHelper.Deserialize<CtAuthorize>("json", content);
            return result;
        }

        public static TResponse Excute<TResponse>(ICtripRequest<TResponse> request) where TResponse : ICtripResponse
        {
            string key = "CtripToken";
            var token = CacheHelper.GetCache(key);//先读取缓存
            if (token == null)//如果没有该缓存,默认为1
            {
                var tk = GetToken();
                token = tk.Access_Token;
                CacheHelper.SetCache(key, tk.Access_Token, tk.Expires_In);//添加缓存
            }

            StringBuilder sbData = new StringBuilder();
            sbData.AppendFormat("AID={0}&", Config.AllianceID);
            sbData.AppendFormat("SID={0}&", Config.SID);
            sbData.AppendFormat("ICODE={0}&", request.ICODE);
            sbData.AppendFormat("UUID={0}&", Config.UID);
            sbData.AppendFormat("Token={0}&", token);
            sbData.Append("mode=1&");
            sbData.Append("format=json");

            var model = new RequestModel(HttpVerbs.POST);
            model.Url = string.Format("http://openservice.ctrip.com/openservice/serviceproxy.ashx?{0}", sbData.ToString());
            model.Data = JsonHelper.Searilize(request);
            model.ContentType = "application/json";
            var content = HttpUtility.PostHtml(model);

            try
            {
                var logWriter = new LogWriter("log/Ctrip/CreatePromotionOrder");
                if (request.GetType() == typeof(CreateOrderRequest))
                {
                    var cRequest = request as CreateOrderRequest;
                    var orderId = cRequest.UniqueID.Where(u => u.Type == "504").FirstOrDefault().ID;
                    string message = $"请求参数：{model.Data}\r\n 响应：{content}";
                    logWriter.WriteOrder(orderId, message);
                }
            }
            catch (Exception ex)
            {

            }


            var result = JsonHelper.Deserialize<TResponse>("json", content);

            //if (result.ResponseStatus == null)
            //{
            //    Thread.Sleep(300);
            //    RedisUilitly.Delete(key);
            //    return Excute(request);
            //}


            return result;
        }
    }
}
