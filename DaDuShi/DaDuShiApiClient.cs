using DaDuShi.Common;
using DaDuShi.Entities;
using DaDuShi.Response;
using FlyPig.Order.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace DaDuShi
{
    public class DaDuShiApiClient
    {
        private readonly LogWriter logWriter;
        public DaDuShiApiClient() : this(DaDuShiConfigManager.RenNiXing)
        {
        }

        public DaDuShiApiClient(DaDuShiConfig config)
        {
            this.Config = config;
            logWriter = new LogWriter("DaDuShi/request");
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
        public DaDuShiConfig Config { get; set; }

        #region Web服务操作对象（订单）  OrderDataOpEt
        public OrderDataOperate _OrderDataOpEt;
        /// <summary>
        /// Web服务操作对象（订单）
        /// </summary>
        public OrderDataOperate OrderDataOpEt
        {
            get
            {
                if (_OrderDataOpEt == null)
                    _OrderDataOpEt = new OrderDataOperate();

                return _OrderDataOpEt;
            }
        }
        #endregion


        #region 统一请求 统一流程处理结果
        /// <summary>
        /// 统一请求 统一流程处理结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="opCode"></param>
        /// <param name="requestAction"></param>
        /// <param name="RequestEt"></param>
        /// <returns></returns>
        public string RequestData<T>(OperationCode opCode, object RequestEt, Func<WebServiceResponse<T>> requestAction)
        {
            WebServiceResponse<T> jsonEt = null;
            string errMsg = "";
            bool isSubmit = false;

            try
            {
                // 请求数据
                jsonEt = requestAction();
                if (jsonEt.Successful)
                {
                    isSubmit = true;
                }
                else
                {
                    string ErrCode;
                    // 获取失败
                    try { ErrCode = ((int)jsonEt.ErrCode).ToString(); }
                    catch { ErrCode = jsonEt.ErrCode.ToString(); }

                    errMsg = string.Format("Web接口报错 ErrCode：{0}   ErrMsg：{1}", ErrCode, jsonEt.ErrMsg.ToString());
                }
            }
            catch (Exception ex)
            {
                jsonEt = null;
                // 报错 记录日志
                errMsg = string.Format("操作异常：{0}", ex.Message);
            }

            // 判断是否需要保存错误日志
            if (!isSubmit)
            {
                // 保存操作日志
                //BLL.Log.Initialize().SaveErrLog(opCode, errMsg, "", JsonUtil.GetJsonByObj(RequestEt));
            }

            return RetrnJson(jsonEt);
        }
        #endregion


        #region 接口返回Json数据 统一处理
        /// <summary>
        /// 接口返回Json数据 统一处理
        /// </summary>
        /// <returns></returns>
        protected string RetrnJson<T>(WebServiceResponse<T> objDataEt)
        {
            dynamic d = new System.Dynamic.ExpandoObject();
            RequestCode code = RequestCode.成功;
            string Msg = "";
            if (objDataEt != null)
            {
                if (!objDataEt.Successful)
                {
                    // 属于 接口供应商的错误代码
                    switch (objDataEt.ErrCode)
                    {
                        case APIErrCode.用户认证失败:
                            code = RequestCode.API_用户认证失败;
                            break;
                        case APIErrCode.内部程序出错:
                            code = RequestCode.API_内部程序出错;
                            break;
                        case APIErrCode.订单号已存在:
                            code = RequestCode.API_订单号已存在;
                            break;
                        case APIErrCode.价格不正确:
                            code = RequestCode.API_价格不正确;
                            break;
                        case APIErrCode.API新错误代码:
                            code = RequestCode.API_新错误代码;
                            Msg = objDataEt.ErrMsg;
                            break;
                        default:
                            code = RequestCode.API_新错误代码;
                            Msg = string.Format("API_新错误代码:{0}", objDataEt.ErrMsg);
                            break;
                    }
                }
                else
                {
                    if (objDataEt.ResponseEt == null)
                        code = RequestCode.无数据返回;
                }
            }
            else
            {
                code = RequestCode.程序代码出错;
            }

            d.Code = (int)code;
            if (string.IsNullOrEmpty(Msg))
                d.Msg = code.ToString();

            if (objDataEt != null)
                d.DataModel = objDataEt.ResponseEt;
            else
            {
                d.DataModel = null;
            }

            return JsonUtil.GetJsonByObj(d);
        }
        #endregion

    }
}
