using System;
using System.Threading.Tasks;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Hotel.Channel;
using Flypig.Order.Application.Order.Entities;
using Flypig.Order.Application.Order.Entities.Dto;

namespace FlyPig.Order.Application.Order.Notice
{
    /// <summary>
    /// 支付通知回调
    /// </summary>
    public class PaySuccessNotice : BaseOrderNotice<PaySuccessRQ, PaySuccessRQResult>
    {

        public PaySuccessNotice(ShopType shop) : base(shop)
        {
        }

        protected override string ChannelDiscernmentCode => Request.OrderId;

        protected override PaySuccessRQResult Notice()
        {
            var result = new PaySuccessRQResult();
            result.ResultCode = "-400";
            result.Message = "XML解析异常";

            int orderState = 1;
            if (Request != null && Request.TaoBaoOrderId > 0)
            {
                if (Channel != ProductChannel.BigTree)
                {
                    orderState = 24;    //订单状态已付款
                }

                var tempOrder = orderRepository.GetSimpleOrderById(Request.TaoBaoOrderId);
                if (tempOrder != null && tempOrder.Aid > 0)
                {
                    if (tempOrder.AlipayPay == 0)
                    {
                        NoticePayOrderDto payDto = new NoticePayOrderDto();
                        payDto.OperatType = NoticeOrderOperatType.Pay;

                        int oldstate = (int)tempOrder.Status;
                        payDto.TaoBaoOrderId = Request.TaoBaoOrderId;
                        payDto.AlipayTradeNo = Request.AlipayTradeNo;
                        payDto.TradeStatus = "WAIT_SELLER_SEND_GOODS";//已冻结/已付款 -> 等待卖家发货 -> 等待卖家确认
                        payDto.AlipayPay = Convert.ToDecimal(Request.Payment) / 100;
                        payDto.State = orderState; //oldstate == 7 ? oldstate : 24;
                        payDto.Remark = string.Format("{2} 【系统】:接收天猫支付通知成功-金额{1}分 [{0}]", DateTime.Now.ToString(), Request.Payment, tempOrder.Remark);
                        var flag = orderRepository.UpdateNoticeOrder(payDto);
                        if (flag)
                        {
                            //异步下单到第三方
                            Task.Factory.StartNew(() =>
                            {
                                    orderRepository.UpdateOrderDelayTime(payDto.TaoBaoOrderId, GetDelayTime(tempOrder.CheckIn));
                                    var orderChannel = ProductChannelFactory.GetOrderChannelByOrderType(tempOrder.OrderType, Shop);
                                    orderChannel.CreateOrder(Convert.ToInt32(tempOrder.Aid));
                            });

                            result.Message = "";
                            result.ResultCode = "0";
                        }
                        else
                        {
                            result.ResultCode = "-400";
                            result.Message = "支付通知保存失败";
                            requestLogWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "【支付通知异常】淘宝支付回调更新失败：支付通知保存失败", Request.TaoBaoOrderId, Request.OrderId);
                        }
                    }
                    else
                    {
                        result.Message = "收到支付通知";
                        result.ResultCode = "0";
                    }
                }
                else
                {
                    result.ResultCode = "-400";
                    result.Message = "不存在该单号";
                    requestLogWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "【支付通知异常】淘宝支付回调更新失败：无法查询到单号");
                }
            }
            return result;
        }


        /// <summary>
        /// 获取超时时间
        /// </summary>
        /// <param name="checkInTime"></param>
        /// <returns></returns>
        public DateTime GetDelayTime(DateTime checkInTime)
        {
            DateTime nowDate = DateTime.Now;
            bool isToDay = checkInTime.DayOfYear == DateTime.Now.DayOfYear;
            int addMinutes = isToDay ? 30 : 60;
            var delayTime = DateTime.Now.AddMinutes(addMinutes);    //当前超时时间

            if (nowDate.Hour < 8)
            {
                var delayNewTime = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, 8, 0, 0);
                delayTime = delayNewTime.AddMinutes(addMinutes);   //9点后超时时间
            }

            return delayTime;
        }
    }
}
