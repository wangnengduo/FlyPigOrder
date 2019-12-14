using AutoMapper;
using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order.Entities.Dto;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Repository.Order
{
    /// <summary>
    /// 自签订单仓储层
    /// </summary>
    public class BigTreeOrderRepository : IOrderRepository
    {
        public NoticeSimpleOrderDto GetSimpleOrderById(long taobaoOrderId)
        {
            return SqlSugarContext.BigTreeInstance.Queryable<tb_hotelorder>().Where(u => u.taoBaoOrderId == taobaoOrderId).Select(u => new NoticeSimpleOrderDto
            {
                Aid = u.aId,
                AlipayPay = u.alipayPay,
                OrderType = u.orderType,
                Remark = u.remark,
                SourceOrderId = u.sourceOrderID,
                Status = (int)u.state,
                CheckIn = u.checkInDate,
                LogisticsStatus = u.logisticsStatus,
                OrderTime = u.orderTime
            }).First();
        }

        public bool IsExists(long taobaoOrderId)
        {
            return SqlSugarContext.BigTreeInstance.Queryable<tb_hotelorder>().Where(u => u.taoBaoOrderId == taobaoOrderId).Any();
        }

        /// <summary>
        /// 保存订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ServiceResult SaveOrder(ProductChannel channel, TmallOrderDto order)
        {
            var result = new ServiceResult();
            try
            {
                var tbOrder = new tb_hotelorder();
                Mapper.Map(order, tbOrder);
                tbOrder.orderType = 2;
                var excuteRows = SqlSugarContext.BigTreeInstance.Insertable(tbOrder).ExecuteCommand();
                if (excuteRows == 0)
                {
                    excuteRows = SqlSugarContext.BigTreeInstance.Insertable(tbOrder).ExecuteCommand();
                }

                if (excuteRows > 0)
                {
                    return result.SetSucess("保存订单成功");
                }
                else
                {
                    return result.SetSucess("保存订单失败");
                }
            }
            catch (Exception ex)
            {
                return result.SetError("保存订单失败：{0}", ex.Message);
            }
        }

        public bool UpdateNoticeOrder(NoticeCommonOrderDto dto)
        {
            var order = SqlSugarContext.BigTreeInstance.Queryable<tb_hotelorder>().Where(u => u.taoBaoOrderId == dto.TaoBaoOrderId).First();
            if (order == null)
            {
                return false;
            }

            if (dto.OperatType == NoticeOrderOperatType.Pay)
            {
                var payDto = dto as NoticePayOrderDto;
                order.alipayTradeNo = payDto.AlipayTradeNo;
                order.tradeStatus = "WAIT_SELLER_SEND_GOODS";//已冻结/已付款 -> 等待卖家发货 -> 等待卖家确认
                order.alipayPay = payDto.AlipayPay;
                order.state = payDto.State;
                order.remark = string.Format("【系统】:接收天猫支付通知成功-金额{1}分 [{0}]<br/> ", DateTime.Now.ToString(), order.remark);
            }
            else if (dto.OperatType == NoticeOrderOperatType.Refund)
            {
                var refundlDto = dto as NoticeRefundOrderDto;
                order.Refuse = 2;
                order.remark = string.Format("【系统】:淘宝申请退款 [{0}]<br/>{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), order.remark);
                order.state = refundlDto.State;
            }
            else if (dto.OperatType == NoticeOrderOperatType.Cancel)
            {
                var cancelDto = dto as NoticeCancelOrderDto;
                order.state = cancelDto.State;
                order.remark = string.Format("{0} 【系统】:淘宝执行订单取消，原因：{2}      [{1}]<br/>", order.remark, DateTime.Now.ToString(), cancelDto.Reason);
            }
            else if (dto.OperatType == NoticeOrderOperatType.NotCenter)
            {
                var notCenterDto = dto as NoticeCancelOrderDto;
                order.state = 22;
                order.modifiedTime = DateTime.Now;
            }

            var excuteRow = SqlSugarContext.BigTreeInstance.Updateable(order).ExecuteCommand();
            return excuteRow > 0;
        }

        public bool UpdateOrderCaoZuo(long aid, string caozuo)
        {
            string sql = string.Format("UPDATE dbo.TB_hotelorder SET caozuo=caozuo+'{1}'+'\r'   WHERE aid={0}", aid, caozuo);
            return SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql) > 0;

        }

        public bool UpdateOrderRemark(long aid, string remark)
        {
            string sql = string.Format("UPDATE dbo.TB_hotelorder SET remark=remark+'<br/>{1}'   WHERE aid={0}", aid, remark);
            return SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql) > 0;
        }

        public bool UpdateOrderRemarkAndSourceOrderID(long aid , int status,string remark,string sourceOrderID)
        {
            string sql = string.Format("UPDATE dbo.TB_hotelorder SET remark=remark+'<br/>{1}',sourceOrderID='{2}',state={3}   WHERE aid={0}", aid, remark, sourceOrderID,status);
            return SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql) > 0;
        }


        /// <summary>
        /// 获取短信内容
        /// </summary>
        /// <param name="shopType"></param>
        /// <param name="source"></param>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        public List<string> GetMessage(int shopType, int source, long taobaoOrderId)
        {
            string sql = string.Format("SELECT * FROM AliTripSMS WHERE taobaoorderId={0} ", taobaoOrderId);
            var messgaeList = SqlSugarContext.BigTreeInstance.Queryable<AliTripSMS>().Where(u => u.TaoBaoOrderId == taobaoOrderId).ToList();
            return messgaeList.Select(u => string.Format("[{0}]  {1}", u.CreateTime, u.Content)).ToList();
        }

        public bool UpdateShipRemark(long aid, int status, string remark)
        {
            string sql = string.Empty;
            if (status == 6)
            {
                sql = string.Format("UPDATE  tb_hotelorder SET remark=remark+'{0}',tradeStatus='WAIT_SELLER_SEND_GOODS',logisticsStatus='STATUS_CONSIGNED',modifiedTime=GETDATE() WHERE taobaoorderid={1}", remark, aid);
            }
            else
            {
                sql = string.Format("UPDATE tb_hotelorder  SET remark=remark+'{0}',modifiedTime=GETDATE(),state={2}  WHERE taobaoorderid={1}", remark, aid, status);
            }
            return SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql) > 0;
        }

        public bool UpdateRemarkState(long aid, int status, string remark)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder  SET remark=remark+'{0}',modifiedTime=GETDATE(),state={2}  WHERE aid={1}", remark, aid, status);


            return SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql) > 0;
        }

        public bool UpdateOrderDelayTime(long taobaoOrderId, DateTime delayTime)
        {
            string sql = string.Format("UPDATE tb_hotelorder SET serviceTimeount = '{0}'  WHERE  taoBaoOrderId ={1}", delayTime.ToString(), taobaoOrderId);
            return SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql) > 0;
        }

        #region 更新状态
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="stutas"></param>
        /// <returns></returns>
        public bool UpdateOrderStutas(long aid, int stutas)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder SET state={1}  WHERE aid={0}", aid, stutas);
            return SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql) > 0;
        }
        #endregion

        #region  更新备注、状态及操作
        /// <summary>
        /// 更新备注、状态及操作
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="caozuo"></param>
        /// <returns></returns>
        public bool UpdateRemarkStateCaoZuo(long aid, int status, string remark, string caozuo)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder  SET remark=remark+'{0}',modifiedTime=GETDATE(),state={2},caozuo=caozuo+'{3}'  WHERE aid={1}", remark, aid, status, caozuo);


            return SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql) > 0;
        }
        #endregion
    }
}
