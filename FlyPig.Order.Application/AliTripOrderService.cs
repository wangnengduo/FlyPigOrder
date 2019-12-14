using Flypig.Order.Application.Order.Entities;
using Flypig.Order.Application.Order.Message;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Common.Tmall;
using FlyPig.Order.Application.Hotel.Channel;
using FlyPig.Order.Application.Repository.Order;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Top.Api.Request;
 
using FlyPig.Order.Application.Order.Notice;

namespace Flypig.Order.Application.Order
{
    public class AliTripOrderService
    {
       // private readonly LogWriter shipLog;
        public AliTripOrderService() : this(ShopType.RenNiXing)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shop"></param>
        public AliTripOrderService(ShopType shop)
        {
            Shop = shop;
           // shipLog = new LogWriter("Tmall/Ship");
        }

        /// <summary>
        /// 店铺
        /// </summary>
        public ShopType Shop { get; set; }

        #region 更新订单状态

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public string UpdateOrderStatus(long orderId, int status, int channel)
        {
            IOrderRepository orderRepository = ProductChannelFactory.CreateOrderRepository(Shop, (ProductChannel)channel);
            var order = orderRepository.GetSimpleOrderById(orderId);
            string remark = string.Format("<br/>【飞猪】:该订单{1} [{0}]", DateTime.Now.ToString(), ((OrderStatus)status).GetDescription());
            if (order != null)
            {
                if (!string.IsNullOrEmpty(order.Remark) && order.Remark.Contains("该订单已确认发货"))
                {
                    orderRepository.UpdateShipRemark(orderId, 6, remark);
                    return "更新订单成功，该订单已发货";
                }

                bool isSuccess = false;
                string message = string.Empty;
                isSuccess = UpdateTmallOrderStatus(orderId, status, out message);
                if (isSuccess)
                {
                    Task.Factory.StartNew(() =>
                    {
                        if (status == 6)
                        {
                            FlyPigMessageUility.ThirdCenterMessage(Shop, order.Aid);
                        }
                    });

                    try
                    {
                        var flag = orderRepository.UpdateShipRemark(orderId, status, remark);
                        if (flag)
                        {
                          //  shipLog.WriteOrder(orderId.ToString(), string.Format("【更新订单状态成功】:订单{0}", orderId));
                            //  return "更新订单状态成功！";
                        }
                        else
                        {
                          //  shipLog.WriteOrder(orderId.ToString(), string.Format("【更新订单状态成功，保存数据库失败】:订单{0}", orderId));
       
                        }
                    }
                    catch (Exception ex)
                    {
                       // shipLog.WriteOrder(orderId.ToString(), string.Format("【更新订单状态出现异常，保存数据库失败】:具体异常:{0}", ex.ToString()));
                    }


                    return "更新订单状态成功！";
                }
                else
                {
                    try
                    {

                        if ((ProductChannel)channel == ProductChannel.BigTree)
                        {
                            var orderInfo = new AliTripOrderClient(Shop).GetOrderStatus(orderId);
                            if (orderInfo != null && orderInfo.TradeStatus == "WAIT_SELLER_SEND_GOODS" && orderInfo.LogisticsStatus == "2")
                            {
                                orderRepository.UpdateShipRemark(orderId, 6, remark);
                                return "更新订单成功，该订单已发货";
                            }
                        }


                        string opration = status == 6 ? "发货失败" : "不确认失败";
                        remark = string.Format("<br/>【飞猪】:该订单 {1}，具体原因：{2} [{0}]", DateTime.Now.ToString(), opration, message);
                        var flag = orderRepository.UpdateShipRemark(orderId, 7, remark);
                    }
                    catch (Exception ex)
                    {
                     //   shipLog.WriteOrder(orderId.ToString(), string.Format("【更新订单状态出现异常】:具体异常:{0}", ex.ToString()));
                    }


                    return "更新订单状态失败！";
                }
            }
            else
            {
                return "订单不存在！";
            }
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool UpdateTmallOrderStatus(long taobaoOrderId, int status, out string message)
        {
            var tmallApiClient = new TmallApiClient(Shop);
            var req = new XhotelOrderUpdateRequest();
            req.Tid = taobaoOrderId;
            if (status == (int)OrderStatus.Confirm)
            {
                req.OptType = 2;
            }
            else if (status == (int)OrderStatus.NotSure)
            {
                req.OptType = 1;
            }


            var rsp = tmallApiClient.Execute(req);
            message = rsp.SubErrMsg;

          //  shipLog.WriteOrder(taobaoOrderId.ToString(), "【更新订单状态返回结果】：{0}", JsonHelper.SerializeObject(rsp));
            if (!string.IsNullOrEmpty(rsp.Result) && rsp.Result == "success")
            {
                return true;
            }
            else
            {
                rsp = tmallApiClient.Execute(req);
                message = rsp.SubErrMsg;
              //  shipLog.WriteOrder(taobaoOrderId.ToString(), "【更新订单状态返回结果】：{0}", JsonHelper.SerializeObject(rsp));

                if (!string.IsNullOrEmpty(rsp.Result) && rsp.Result == "success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        #endregion

        #region 执行订单处理
        /// <summary>
        /// 执行订单处理
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public TaoBaoResultXml ExcuteOrder(string xml)
        {
            TaoBaoResultXml result = new TaoBaoResultXml();
            if (string.IsNullOrEmpty(xml))
            {
                result.Message = "请求参数有误";
                result.ResultCode = "-998";
                return result;
            }

            try
            {
                var orderNotice = GetOrderNotice(xml);
                result = orderNotice.ExcuteOrder(xml);
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "系统异常:" + ex.Message.ToString();
                result.ResultCode = "-998";
                return result;
            }
        }
        #endregion

        #region 获取通知类型
        /// <summary>
        /// 获取通知类型
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        IOrderNotice GetOrderNotice(string xml)

        {
            try
            {
                if (xml.Contains("<ValidateRQ>"))
                    return new ValidateNotice(Shop);
                else if (xml.Contains("<BookRQ>"))
                    return new BookNotice(Shop);
                else if (xml.Contains("<QueryStatusRQ>"))
                    return new QueryStatusNotice(Shop);
                else if (xml.Contains("<PaySuccessRQ>"))
                    return new PaySuccessNotice(Shop);
                else if (xml.Contains("<OrderRefundRQ>"))
                    return new RefundNotice(Shop);
                else if (xml.Contains("<CancelRQ>"))
                    return new CancelNotice(Shop);
                else if (xml.Contains("<UrgeRQ>"))
                    return new UrgeNotice(Shop);
                else return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region 获取渠道信息
        /// <summary>
        /// 根据价格计划获取渠道
        /// </summary>
        /// <param name="ratePlanCode"></param>
        /// <returns></returns>
        public static ProductChannel GetChannelByRPCode(string ratePlanCode)
        {
            if (ratePlanCode.Substring(0, 2).Contains("mr") || ratePlanCode.Substring(0, 2).Contains("rm"))
                return ProductChannel.MT;
            else if (ratePlanCode.Substring(0, 2).Contains("xr"))
                return ProductChannel.Ctrip;
            else if (ratePlanCode.Substring(0, 2).Contains("dr"))
                return ProductChannel.DDS;
            else
                throw new ArgumentException("订单号有问题，不存在此格式订单号");

            /*if (ratePlanCode.Contains("wl") || ratePlanCode.Contains("yj") || ratePlanCode.Contains("rz") || ratePlanCode.Contains("yg") || ratePlanCode.Contains("mb"))
                return ProductChannel.BigTree;
            else if (ratePlanCode.Contains("h"))
                return ProductChannel.Hds;
            else if (ratePlanCode.Contains("rt"))
                return ProductChannel.LY;
            else if (ratePlanCode.Contains("el") || ratePlanCode.Contains("ey"))
                return ProductChannel.Elong;
            else if (ratePlanCode.Contains("mt") || ratePlanCode.Contains("tx") || ratePlanCode.Contains("ms") || ratePlanCode.Contains("mr")
                || ratePlanCode.Contains("rm") || ratePlanCode.Contains("nv") || ratePlanCode.Contains("ym"))
                return ProductChannel.MT;
            else if (ratePlanCode.Contains("jl"))
                return ProductChannel.JL;
            else if (ratePlanCode.Contains("xl"))
                return ProductChannel.XW;
            else if (ratePlanCode.Contains("ct") || ratePlanCode.Contains("rc") || ratePlanCode.Contains("yc") || ratePlanCode.Contains("cc") || ratePlanCode.Contains("mc") || ratePlanCode.Contains("xr"))
                return ProductChannel.Ctrip;
            else
                throw new ArgumentException("不存在此格式的价格计划");*/
        }

        /// <summary>
        /// 根据订单号获取渠道
        /// </summary>
        /// <param name="ratePlanCode"></param>
        /// <returns></returns>
        public static ProductChannel GetChannelByOrderId(string orderId)
        {
            if (orderId.Contains("mr") || orderId.Contains("rm"))
                return ProductChannel.MT;
            else if (orderId.Contains("xr"))
                return ProductChannel.Ctrip;
            else if (orderId.Contains("dr"))
                return ProductChannel.DDS;
            else
                throw new ArgumentException("订单号有问题，不存在此格式订单号");

            /*if (orderId.Contains("w") || orderId.Contains("Y") || orderId.Contains("RF") || orderId.Contains("yg") || orderId.Contains("mb"))
                return ProductChannel.BigTree;
            else if (orderId.Contains("q"))
                return ProductChannel.Hds;
            else if (orderId.Contains("t"))
                return ProductChannel.LY;
            else if (orderId.Contains("E") || orderId.Contains("EL"))
                return ProductChannel.Elong;
            else if (orderId.Contains("m") || orderId.Contains("ym") || orderId.Contains("ms") || orderId.Contains("mr") || orderId.Contains("rm"))
                return ProductChannel.MT;
            else if (orderId.Contains("mn"))
                return ProductChannel.MTNew;
            else if (orderId.Contains("J"))
                return ProductChannel.JL;
            else if (orderId.Contains("xl"))
                return ProductChannel.XW;
            else if (orderId.Contains("c") || orderId.Contains("rc") || orderId.Contains("yc") || orderId.Contains("cs") || orderId.Contains("xr"))
                return ProductChannel.Ctrip;
            else
                throw new ArgumentException("订单号有问题，不存在此格式订单号");*/
        }

        #endregion



    }
}
