using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FlyPig.Order.Application.Common;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using SqlSugar;
using Flypig.Order.Application.Order.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;
using Flypig.Order.Application.Common;
using FlyPig.Order.Application.Entities.Dto;
using Newtonsoft.Json;
using DaDuShi.Request;
using DaDuShi.Response;
using DaDuShi.Entities;
using FlyPig.Order.Framework.Logging;

namespace FlyPig.Order.Application.Repository.Order
{
    /// <summary>
    /// 天猫第三方
    /// </summary>
    public class ThirdOrderRepository : IOrderRepository
    {
        protected SqlSugarClient sqlSugarClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ThirdOrderRepository() : this(SqlSugarContext.ResellbaseInstance)
        {
        }
        private readonly LogWriter orderStatuslogWriter;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sqlSugar"></param>
        public ThirdOrderRepository(SqlSugarClient sqlSugar)
        {
            sqlSugarClient = sqlSugar;
            orderStatuslogWriter = new LogWriter("DDS/orderStatus/");
        }

        #region 根据订单Aid获取订单
        /// <summary>
        /// 根据订单Aid获取订单
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public TB_hotelcashorder GetOrderByAid(int aid)
        {
            string sql = string.Format("select * from TB_hotelcashorder with(nolock) where aid={0}", aid);
            return sqlSugarClient.SqlQueryable<TB_hotelcashorder>(sql).First();
        }
        #endregion

        #region 根据淘宝单号获取订单
        /// <summary>
        /// 根据淘宝单号获取订单
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        public TB_hotelcashorder GetOrderByTaoBaoOrderId(long taobaoOrderId)
        {
            string sql = string.Format("select * from TB_hotelcashorder with(nolock) where taobaoorderid={0}", taobaoOrderId);
            return sqlSugarClient.SqlQueryable<TB_hotelcashorder>(sql).First();
        }
        #endregion

        #region 获取订单简要信息
        /// <summary>
        /// 获取订单简要信息
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        public NoticeSimpleOrderDto GetSimpleOrderById(long taobaoOrderId)
        {
            var orderInfo = sqlSugarClient.Queryable<TB_hotelcashorder>().Where(u => u.taoBaoOrderId == taobaoOrderId)
                   .Select(u => new NoticeSimpleOrderDto
                   {
                       Aid = u.aId,
                       AlipayPay = u.alipayPay,
                       OrderType = u.orderType,
                       Remark = u.remark,
                       SourceOrderId = u.sourceOrderID,
                       Status = u.state,
                       CheckIn = u.checkInDate,
                       LogisticsStatus = u.logisticsStatus,
                       OrderTime = u.orderTime,
                       ContactTel=u.contactTel
                   }).First();

            return orderInfo;
        }
        #endregion

        #region 是否存在订单
        /// <summary>
        /// 是否存在订单
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        public bool IsExists(long taobaoOrderId)
        {
            return sqlSugarClient.Queryable<TB_hotelcashorder>().Where(u => u.taoBaoOrderId == taobaoOrderId).Any();
        }
        #endregion

        #region 保存订单
        /// <summary>
        /// 保存订单
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ServiceResult SaveOrder(ProductChannel channel, TmallOrderDto order)
        {
            var result = new ServiceResult();
            try
            {
                var tbOrder = new TB_hotelcashorder();
                Mapper.Map(order, tbOrder);
                tbOrder.orderCheckoutDate = new DateTime(1990, 1, 1);

                var shop = (ShopType)order.shopType;
                if (channel == ProductChannel.MT)
                {
                    if (shop == ShopType.ShengLv)
                    {
                        tbOrder.orderType = 11;
                    }
                    else if (shop == ShopType.RenNiXing)
                    {
                        tbOrder.orderType = 14;
                    }
                    else
                    {
                        tbOrder.orderType = 13;
                    }
                }
                else if (channel == ProductChannel.Ctrip)
                {
                    if (order.RatePlanCode.Contains("_c"))
                    {
                        tbOrder.orderType = 16;
                    }
                    else
                    {
                        //15 喜玩携程    5辰亿
                        tbOrder.orderType = CtripOrderType();
                    }
                    //if (shop == ShopType.LingZhong)
                    //{
                    //    tbOrder.orderType = 5;
                    //}
                    //else
                    //{
                    //    tbOrder.orderType = 15;
                    //}
                }
                else if (channel == ProductChannel.DDS)
                {
                    //大都市产品
                    tbOrder.orderType = 17;
                }
                else
                {
                    tbOrder.orderType = Int16.Parse(channel.GetDescription());
                }

                tbOrder.serviceTimeount = new DateTime(1990, 1, 1);       // GetDelayTime(dto.checkInDate);
                tbOrder.prePay = 1;

                if (channel == ProductChannel.Elong)
                {
                    tbOrder.source = "elong";
                }
                else if (channel == ProductChannel.LY)
                {
                    tbOrder.source = "tclj";
                }
                tbOrder.Refuse = 0;   //退款标记

                var isSuccess = sqlSugarClient.Insertable(tbOrder).ExecuteCommand() > 0;
                if (isSuccess)
                {
                    return result.SetSucess("保存订单成功");
                }
                else
                {
                    return result.SetError("保存订单失败");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 更新订单通知
        /// <summary>
        /// 更新订单通知
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool UpdateNoticeOrder(NoticeCommonOrderDto dto)
        {
            var order = sqlSugarClient.Queryable<TB_hotelcashorder>().Where(u => u.taoBaoOrderId == dto.TaoBaoOrderId).First();
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
                order.remark = string.Format("【系统】:接收天猫支付通知成功-金额{1}分 [{0}]<br/>{2} ", DateTime.Now.ToString(), order.alipayPay, order.remark);
            }
            else if (dto.OperatType == NoticeOrderOperatType.Refund)
            {
                var refundlDto = dto as NoticeRefundOrderDto;
                order.Refuse = 2;
                order.remark = string.Format("{1}【系统】:淘宝申请退款 [{0}] <br/>", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), order.remark);
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

            var excuteRow = sqlSugarClient.Updateable(order).ExecuteCommand();
            return excuteRow > 0;
        }
        #endregion

        #region 更新订单操作
        /// <summary>
        /// 更新订单操作
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="caozuo"></param>
        /// <returns></returns>
        public bool UpdateOrderCaoZuo(long aid, string caozuo)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder SET caozuo=caozuo+'{1}\n'   WHERE aid={0}", aid, caozuo);
            return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
        }
        #endregion

        #region 更新订单备注
        /// <summary>
        /// 更新订单备注
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateOrderRemark(long aid, string remark)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder SET remark=remark+'{1}<br/>'   WHERE aid={0}", aid, remark);
            return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
        }
        #endregion

        #region 更新订单备注及渠道订单号
        /// <summary>
        /// 更新订单备注
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateOrderRemarkAndSourceOrderID(long aid,int status, string remark,string sourceOrderID)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder SET remark=remark+'{1}<br/>',sourceOrderID='{2}',state={3}   WHERE aid={0}", aid, remark, sourceOrderID, status);
            return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
        }
        #endregion

        #region 更新发货备注
        /// <summary>
        /// 更新发货备注
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateShipRemark(long aid, int status, string remark)
        {
            string sql = string.Empty;
            if (status == 6)
            {
                sql = string.Format("UPDATE TB_hotelcashorder   SET remark=remark+'{0}',tradeStatus='WAIT_SELLER_SEND_GOODS',logisticsStatus='STATUS_CONSIGNED',modifiedTime=GETDATE(),cofirmorderdate=getdate(),state=6  WHERE taobaoorderid={1}", remark, aid);
            }
            else
            {
                sql = string.Format("UPDATE  TB_hotelcashorder  SET remark=remark+'{0}',modifiedTime=GETDATE(),state={2}  WHERE taobaoorderid={1}", remark, aid, status);
            }

            if (sqlSugarClient.Ado.ExecuteCommand(sql) == 0)
            {
                return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 更新备注及状态
        /// <summary>
        /// 更新备注及状态
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateRemarkState(long aid, int status, string remark)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder  SET remark=remark+'{0}<br/>',modifiedTime=GETDATE(),state={2}  WHERE aid={1}", remark, aid, status);
            

            if (sqlSugarClient.Ado.ExecuteCommand(sql) == 0)
            {
                return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 更新备注、状态及操作
        /// <summary>
        /// 更新备注及状态
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateRemarkStateCaoZuo(long aid, int status, string remark, string caozuo)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder  SET remark=remark+'{0}',modifiedTime=GETDATE(),state={2},caozuo=caozuo+'{3}'  WHERE aid={1}", remark, aid, status, caozuo);


            if (sqlSugarClient.Ado.ExecuteCommand(sql) == 0)
            {
                return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 更新订单超时时间
        /// <summary>
        /// 更新订单超时时间
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        public bool UpdateOrderDelayTime(long taobaoOrderId, DateTime delayTime)
        {
            string sql = string.Format("UPDATE TB_hotelcashorder SET serviceTimeount = '{0}'  WHERE  taoBaoOrderId ={1}", delayTime.ToString(), taobaoOrderId);
            return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
        }
        #endregion

        #region 获取推送入住状态订单
        /// <summary>
        /// 获取推送入住状态订单
        /// </summary>
        /// <returns></returns>
        public List<TB_hotelcashorder> GetPushCheckInStatusOrder()
        {
            string sql = @"select top 10 * from TB_hotelcashorder 
            where  
            orderTime >= DATEADD(DD,-1,getdate()) and logisticsStatus='STATUS_CONSIGNED'
            and caozuo not like '%推送已入住状态%'";
            return sqlSugarClient.SqlQueryable<TB_hotelcashorder>(sql).ToList();
        }
        #endregion

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
            return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
        }
        #endregion

        #region 更新渠道订单号
        /// <summary>
        /// 更新订单备注
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateOrderSourceOrderID(long aid,string sourceOrderID)
        {
            string sql = string.Format("UPDATE  TB_hotelcashorder SET sourceOrderID='{1}'  WHERE aid={0}", aid, sourceOrderID);
            return sqlSugarClient.Ado.ExecuteCommand(sql) > 0;
        }
        #endregion

        //获取携程下单到哪个马甲
        public short CtripOrderType()
        {
            short orderType = 5;
            try
            {
                string sql = string.Format("SELECT Value FROM Dictionary (NOLOCK) where [Key]='OrderConfig'");
                var chDictionary = SqlSugarContext.LingZhongInstance.SqlQueryable<Dictionary>(sql).First().Value;
                orderType = JsonConvert.DeserializeObject<chDictionary>(chDictionary).Ctrip;
            }
            catch (Exception ex)
            {
                
            }
            return orderType;
        }

        public class chDictionary
        {
            public short MeiTuan { get; set; }
            public short Ctrip { get; set; }
        }

        private OrderDataOperate _OrderDataOpEt;
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

        /// <summary>
        /// 获取大都市订单状态
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        public string GetOrderStatusDDS(string taobaoOrderId)
        {
            string StatusName = string.Empty;
            //查询订单请求类
            var request = new OrderRequestJsonEt();
            var response = new WebServiceResponse<OrderQueryJsonEt>();
            try
            {
                var order = SqlSugarContext.ResellbaseInstance.Queryable<TB_hotelcashorder>().Where(u => u.taoBaoOrderId.ToString() == taobaoOrderId).First();
                if (order == null)
                {
                    StatusName = "订单不存在";
                }
                else
                {
                    StatusName = "未知状态";
                    request = new OrderRequestJsonEt
                    {
                        DistributorReservationId = order.orderNO
                    };
                    try
                    {
                        response = OrderDataOpEt.OrderQuery(request);
                    }
                    catch (Exception ex)
                    {
                        response = OrderDataOpEt.OrderQuery(request);
                    }
                    if (response != null && response.ResponseEt != null)
                    {
                        if (string.IsNullOrEmpty(order.sourceOrderID) && !string.IsNullOrEmpty(response.ResponseEt.ProductReservationId))
                        {
                            UpdateOrderSourceOrderID(order.aId, response.ResponseEt.ProductReservationId);
                        }
                        if (response.ResponseEt.ReservationStatus != null && response.ResponseEt.ReservationStatus.ToLower().Contains("confirmed"))
                        {
                            StatusName = "已确认";
                            try
                            {
                                string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                                string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                                string orderText = "查询订单号：" + order.orderNO + "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse + "\r\n" + "------------------------------------------";
                                orderStatuslogWriter.Write(orderText);
                            }
                            catch (Exception exOrder)
                            {
                            }
                        }
                        else if (response.ResponseEt.ReservationStatus.ToLower().Contains("canceled"))
                        {
                            StatusName = "取消";
                            try
                            {
                                string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                                string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                                string orderText = "查询订单号：" + order.orderNO + "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse + "\r\n" + "------------------------------------------";
                                orderStatuslogWriter.Write(orderText);
                            }
                            catch (Exception exOrder)
                            {
                            }
                        }
                        else if (response.ResponseEt.ReservationStatus.ToLower().Contains("pending"))
                        {
                            StatusName = "预订中";
                            //记录查询订单不确定订单日志
                            //try
                            //{
                            //    string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                            //    string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                            //    string orderText = "查询订单号：" + order.orderNO + "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse + "\r\n" + "------------------------------------------";
                            //    orderStatuslogWriter.Write(orderText);
                            //}
                            //catch (Exception exOrder)
                            //{
                            //}
                        }
                        else if (response.ResponseEt.ReservationStatus.ToLower().Contains("failed"))
                        {
                            StatusName = "供应商返回发生错误";
                            //记录查询订单不确定订单日志
                            try
                            {
                                string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                                string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                                string orderText = "查询订单号：" + order.orderNO + "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse + "\r\n" + "------------------------------------------";
                                orderStatuslogWriter.Write(orderText);
                            }
                            catch (Exception exOrder)
                            {
                            }
                        }
                        else
                        {
                            StatusName = "未知状态";
                            //记录查询订单不确定订单日志
                            try
                            {
                                string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                                string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                                string orderText = "查询订单号：" + order.orderNO + "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse + "\r\n" + "------------------------------------------";
                                orderStatuslogWriter.Write(orderText);
                            }
                            catch (Exception exOrder)
                            {
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(order.sourceOrderID))
                        {
                            StatusName = "未下单到大都市";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusName = "查询失败";
                //记录查询订单不确定订单日志
                try
                {
                    string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    string orderText = "查询订单飞猪订单号：" + taobaoOrderId + "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse + "\r\n" + "------------------------------------------";
                    orderStatuslogWriter.Write(orderText);
                }
                catch (Exception exOrder)
                {
                }
            }
            return StatusName;
        }
    }
}
