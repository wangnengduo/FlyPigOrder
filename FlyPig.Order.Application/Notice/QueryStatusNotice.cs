using System;
using System.Linq;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using Flypig.Order.Application.Order.Entities;

namespace FlyPig.Order.Application.Order.Notice
{
    public class QueryStatusNotice : BaseOrderNotice<QueryStatusRQ, QueryStatusResult>
    {
        public QueryStatusNotice(ShopType shop) : base(shop)
        {
        }

        protected override string ChannelDiscernmentCode => Request.OrderId;


        protected override QueryStatusResult Notice()
        {
            QueryStatusResult result = new QueryStatusResult();
            result.ResultCode = "-301";
            result.Message = "解析xml异常";

            if (string.IsNullOrEmpty(Request.OrderId))
            {
                result.ResultCode = "-302";
                result.Message = "订单不存在";
                return result;
            }

            if (Request != null && Request.TaoBaoOrderId > 0)
            {
                var order = orderRepository.GetSimpleOrderById(Request.TaoBaoOrderId);
                if (order != null && order.Aid > 0)
                {
                    result.Message = Request.OrderId;
                    result.ResultCode = "0";
                    result.Status = "3";//默认处理中
                    result.OrderId = Request.OrderId;

                    #region 设置确认号

                    try
                    {
                        if (order.OrderType == 3)   //艺龙
                        {
                            result.PmsResID = order.SourceOrderId;
                            //   result.Comment = "艺龙";
                        }
                        else if (order.OrderType == 11 || order.OrderType == 14)   //盛旅
                        {
                            var channelOrder = SqlSugarContext.ResellbaseInstance.Queryable<dingdan_info>().Where(u => u.fax == Request.TaoBaoOrderId.ToString()).First();
                            result.PmsResID = channelOrder.ElongOrderID;
                            //  result.Comment = "美团";
                        }
                        else if (order.OrderType == 12 || order.OrderType == 13) //辰亿
                        {
                            var channelOrder = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.fax == Request.TaoBaoOrderId.ToString()).First();
                            result.PmsResID = channelOrder.ElongOrderID;
                            //   result.Comment = "美团";
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    #endregion

                    if (order.Status == 2)
                    {
                        result.Status = "4";//取消
                    }
                    else if (order.Status == 22)
                    {
                        result.Status = "2"; //不确认
                    }
                    else if (!string.IsNullOrEmpty(order.Remark) && order.Remark.Contains("该订单已入住"))
                    {
                        result.Status = "5";  //已入住
                    }
                    else if (new int[] { 6, 48, 49, 52, 53, 56 }.Contains(order.Status) || (!string.IsNullOrEmpty(order.Remark)
                        && (order.Remark.Contains("该订单已确认") || order.Remark.Contains("订单当前状态不允许进行该操作"))))
                    {
                        result.Status = "1";///订单已确认
                    }

                    //18点之后订单下单半小时后返回已入住
                    if (order.LogisticsStatus == "STATUS_CONSIGNED" && order.OrderTime.Hour >= 18 && order.CheckIn == DateTime.Now.Date
                        && DateTime.Now.Subtract(order.OrderTime).TotalMinutes >= 30)
                    {
                        result.Status = "5";  //已入住
                    }
                }
                else
                {
                    result.ResultCode = "-302";
                    result.Message = "订单不存在";
                    requestLogWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "【查询订单状态异常】：订单不存在");
                }
            }

            return result;
        }
    }
}
