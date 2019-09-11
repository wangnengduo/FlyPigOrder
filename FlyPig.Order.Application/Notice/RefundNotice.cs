using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Hotel.Channel;
using Flypig.Order.Application.Order.Entities;
using Flypig.Order.Application.Order;
using Flypig.Order.Application.Order.Entities.Dto;

namespace FlyPig.Order.Application.Order.Notice
{
    public class RefundNotice : BaseOrderNotice<OrderRefundRQ, OrderRefundRQResult>
    {

        private IOrderChannel orderChannel;

        public RefundNotice(ShopType shop) : base(shop)
        {
        }

        protected override string ChannelDiscernmentCode => Request.OrderId;

        protected override OrderRefundRQResult Notice()
        {
            OrderRefundRQResult result = new OrderRefundRQResult();
            result.ResultCode = "-400";
            result.Message = "解析xml异常";
            if (Request != null && Request.TaoBaoOrderId > 0)
            {
                var tempOrder = orderRepository.GetSimpleOrderById(Request.TaoBaoOrderId);
                if (tempOrder != null && tempOrder.Aid > 0)
                {
                    NoticeRefundOrderDto refundDto = new NoticeRefundOrderDto();
                    refundDto.OperatType = NoticeOrderOperatType.Refund;
                    refundDto.TaoBaoOrderId = Request.TaoBaoOrderId;
                    int oldState = (int)tempOrder.Status;
                    if (oldState != 22)
                    {
                        refundDto.State = 3;//客服跟进
                    }
                    var flag = orderRepository.UpdateNoticeOrder(refundDto);
                    if (flag)
                    {
                        #region 异步调用取消接口
                        Task.Factory.StartNew(() =>
                        {
                            if (Channel != ProductChannel.BigTree)
                            {
                                var tmallOrder = aliTripOrderClient.GetOrderStatus(Request.TaoBaoOrderId);
                                if (tmallOrder.LogisticsStatus != "TRADE_SUCCESS")
                                {
                                    orderChannel = ProductChannelFactory.GetOrderChannelByOrderType(tempOrder.OrderType, Shop);
                                    orderChannel.CancelOrder(Convert.ToInt32(tempOrder.Aid));
                                }
                            }
                        });
                        #endregion
                        result.Message = "";
                        result.ResultCode = "0";
                    }
                    else
                    {
                        result.ResultCode = "-400";
                        result.Message = "支付通知退款保存失败";
                        requestLogWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "【退款通知异常】淘宝知会退款更新失败：保存数据库更新字段出错。");
                    }
                }
                else
                {
                    result.ResultCode = "-400";
                    result.Message = "不存在该单号";
                    requestLogWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "【退款通知异常】淘宝知会退款更新失败：无法查询到单号。");
                }
            }
            return result;
        }
    }
}
