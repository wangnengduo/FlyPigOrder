using Flypig.Order.Application.Common;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Top.Api;

namespace FlyPig.Order.Application.Common.Tmall
{
    /// <summary>
    /// 天猫API客户端
    /// </summary>
    public class TmallApiClient
    {

        private AliTripConfig _apiConfig;

        public TmallApiClient(ShopType shopType = ShopType.LingZhong)
        {
            this.Url = "http://gw.api.taobao.com/router/rest";
            Shop = shopType;
            _apiConfig = GetApiConfig();
            if (shopType == ShopType.LingZhong)
            {
                UserName = "taobao";
                Password = "taobao";
            }
            else if (shopType == ShopType.YinJi)
            {
                UserName = "yinji";
                Password = "yinji888";
            }
            else if (shopType == ShopType.ShengLv)
            {
                UserName = "shenglv";
                Password = "sltm888888";
            }
            else if (Shop == ShopType.RenXing)
            {
                UserName = "rxflypig";
                Password = "rx123123";
            }
            else
            {
                UserName = "tbqucs";
                Password = "tbqucs123";
            }
        }


        /// <summary>
        /// 店铺
        /// </summary>
        public ShopType Shop { get; set; }

        /// <summary>
        /// 接口网址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 验证用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 验证密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 获取飞猪配置
        /// </summary>
        /// <returns></returns>
        public AliTripConfig GetApiConfig()
        {
            if (_apiConfig != null)
            {
                return _apiConfig;
            }
            //写死配置
            var config = aliTripConfig(Shop);
            //从数据库中获取飞猪配置
            //var config = SqlSugarContext.LingZhongInstance.Queryable<AliTripConfig>().Where(u => u.ShopType == (int)Shop).First();
            return config;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public TResponse Execute<TResponse>(ITopRequest<TResponse> request) where TResponse : TopResponse
        {
            var client = new DefaultTopClient(this.Url, _apiConfig.AppKey, _apiConfig.AppSecret);
            return client.Execute(request, _apiConfig.SessionKey) as TResponse;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ServiceResult Execute<TResponse>(ITopRequest<TResponse> request, Func<TResponse, ServiceResult> callback) where TResponse : TopResponse
        {
            var client = new DefaultTopClient(this.Url, _apiConfig.AppKey, _apiConfig.AppSecret);
            var result = client.Execute(request, _apiConfig.SessionKey) as TResponse;
            return callback.Invoke(result);
        }

        public AliTripConfig aliTripConfig(ShopType shopType = ShopType.RenNiXing)
        {
            var config = new AliTripConfig();
            if (Shop == ShopType.RenNiXing)
            {
                config.Id = 17;
                config.AppName = "贵州任你行酒店专营店";
                config.AppKey = "25316312";
                config.AppSecret = "079b3279307304ed39a6d08ae4aebfd0";
                config.SessionKey = "6100e063fc0b1d1826f1eaccee286b04603a1a1e17e04ed2200535632758";
                config.RefreshToken = "6102b06162cf2f15ac3690ea3bcf6065f27d0bad4ac1b882200535632758 ";
                config.ShopType = (int)shopType;
            }
            else
            {
                config = SqlSugarContext.LingZhongInstance.Queryable<AliTripConfig>().Where(u => u.ShopType == (int)Shop).First();
            }
            return config;
        }
    }
}
