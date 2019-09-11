
using Flypig.Order.Application.Order.Entities;
using Flypig.Order.Application.Order.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;

namespace FlyPig.Order.Application.Order.Notice
{
    public class CancelNotice : BaseOrderNotice<CancelRQ, CancelRQResult>
    {

        public CancelNotice(ShopType shop) : base(shop)
        {
        }

        protected override string ChannelDiscernmentCode => Request.OrderId;

        protected override CancelRQResult Notice()
        {
            var result = new CancelRQResult();
            result.ResultCode = "-200";
            result.Message = "解析xml异常";
            var cq = Request;
            if (cq != null && cq.TaoBaoOrderId > 0)
            {
                InitChannel(cq.OrderId, OrderMarkType.OrderId);

                var tempOrder = orderRepository.GetSimpleOrderById(cq.TaoBaoOrderId);
                if (tempOrder != null && tempOrder.Aid > 0)
                {
                    if (!string.IsNullOrEmpty(tempOrder.Remark) && tempOrder.Remark.Contains("该订单客人已入住"))
                    {
                        result.Message = "当前已入住，无法取消";
                        result.ResultCode = "-206";
                    }
                    else if (!string.IsNullOrEmpty(tempOrder.Remark) && tempOrder.Remark.Contains("该订单已确认") && tempOrder.Remark.Contains("订单当前状态不允许进行该操作"))
                    {
                        result.Message = "当前已确认，无法取消";
                        result.ResultCode = "-206";
                    }
                    else
                    {
                        if (tempOrder.Status != 2)
                        {
                            NoticeCancelOrderDto cancelDto = new NoticeCancelOrderDto();
                            cancelDto.OperatType = NoticeOrderOperatType.Cancel;
                            cancelDto.TaoBaoOrderId = cq.TaoBaoOrderId;
                            int oldStatus = (int)tempOrder.Status;
                            cancelDto.Reason = cq.Reason;

                            if (tempOrder.AlipayPay <= 0 && string.IsNullOrEmpty(tempOrder.SourceOrderId))
                            {
                                cancelDto.State = 2;
                            }
                            else
                            {
                                cancelDto.State = 3;
                                //  cancelDto.State = oldStatus;
                            }

                            var flag = orderRepository.UpdateNoticeOrder(cancelDto);
                            if (!flag)
                            {
                                result.Message = "取消失败";
                                result.ResultCode = "-200";
                                requestLogWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "【取消通知异常】更新取消状态失败，保存数据库出现异常。");
                            }
                            else
                            {
                                result.ResultCode = "0";
                                result.Message = cq.OrderId;
                                result.OrderId = cq.OrderId;
                            }
                        }
                        else
                        {
                            result.Message = "订单已取消";
                            result.ResultCode = "-205";
                            requestLogWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "【取消通知异常】更新取消状态失败，订单已取消。");
                        }
                    }
                }
                else
                {
                    result.Message = "不存在此订单";
                    result.ResultCode = "-204";
                    requestLogWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "【取消通知异常】更新取消状态失败，不存在此订单。");
                }
            }
            return result;
        }


    }
}
