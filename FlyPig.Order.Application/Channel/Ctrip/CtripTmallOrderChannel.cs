
using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.MT.Order.Channel;
using FlyPig.Order.Application.Repository.Channel;
using FlyPig.Order.Application.Repository.Order;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using FlyPig.Order.Framework.Logging;
using SqlSugar;
using System;

namespace LingZhong.HotelManager.Application.Channel.Ctrip.Order
{
    /// <summary>
    /// 携程天猫订单渠道
    /// </summary>
    public class CtripTmallOrderChannel : IOrderChannel
    {
        private readonly LogWriter syncOrderLogWriter;
        private readonly ThirdOrderRepository orderRepository;
        private readonly CtripRepository ctripRepository;

        private readonly ChenYiCtripOrderChannel chenYiCtripOrderChannel;
        private readonly SqlSugarClient sqlSugarClient;
        protected readonly string OrderThirdFailKey;
        protected readonly string OrderDistributionFailKey;

        public CtripTmallOrderChannel() : this(ShopType.RenNiXing)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CtripTmallOrderChannel(ShopType shopType)
        {
            Shop = shopType;
            chenYiCtripOrderChannel = new ChenYiCtripOrderChannel(shopType);
            syncOrderLogWriter = new LogWriter("SynOrder");
            if (Shop == ShopType.ShengLv || Shop == ShopType.RenNiXing)
            {
                sqlSugarClient = SqlSugarContext.ResellbaseInstance;
                orderRepository = new ShengLvOrderRepository();
            }
            else
            {
                orderRepository = new ThirdOrderRepository();
                sqlSugarClient = SqlSugarContext.BigTreeInstance;
            }

            ctripRepository = new CtripRepository();
        }

        public ShopType Shop { get; set; }

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult CancelOrder(int id)
        {
            var result = new ServiceResult();
            try
            {
                var order = orderRepository.GetOrderByAid(id);
                string message = string.Empty;
                bool islimit = true;
                #region 未设置一键退改
                //if (order.shopType == 8)
                //{
                //    var rateplanService = new RatePlanService((ShopType)order.shopType, ProductChannel.MT);
                //    var rpInfo = rateplanService.GetTmallRatePlanByRpId(order.TaoBaoRatePlanId);
                //    var cancelProfix = rpInfo.CancelPolicy;
                //    var taoBaoCancelPolicy = JsonHelper.Deserialize<TaoBaoCancelPolicy>("json", cancelProfix);

                //    var cancelTime = taoBaoCancelPolicy.PolicyInfo.Where(u => u.Value == 0).FirstOrDefault();
                //    if (cancelTime.Key > 0)
                //    {
                //        var canCancelTime = order.checkInDate.Date.AddDays(1);   //可取消时间
                //        canCancelTime = canCancelTime.AddHours(-(cancelTime.Key));
                //        if (DateTime.Now <= canCancelTime)
                //        {
                //            message += string.Format("<br/>【系统】检测该订单在可取消时间范围内 .. [{0}]", DateTime.Now.ToString());
                //            islimit = false;
                //        }
                //    }

                //    if (islimit)
                //    {
                //        if (order.remark.Contains("订单发货成功") || order.remark.Contains("后台已发货") || order.remark.Contains("天猫执行发货成功") || order.remark.Contains("已同步发货状态到本地"))
                //        {
                //            return result.SetError("订单不在可取消范围内，无法提交取消操作到美团");
                //        }
                //    }
                //} 
                #endregion

                if (order.caozuo.Contains("提交取消到携程") && (order.remark.Contains("订单取消成功") || order.remark.Contains("订单取消失败")))
                {
                    return result.SetError("禁止重复提交取消");
                }

                var cancelResult = chenYiCtripOrderChannel.CancelOrder(order.taoBaoOrderId);

                if (cancelResult.IsSucess)
                {
                    order.remark = string.Format("{0}{1}【携程】:订单取消成功  [{2}]<br/>", order.remark, message, DateTime.Now.ToString());
                    order.caozuo = string.Format("{0} 系统提交取消到携程 [{1}]\n", order.caozuo, DateTime.Now.ToString());
                    result.SetSucess("订单取消成功");
                }
                else
                {
                    order.state = 7;
                    order.caozuo = string.Format("{0}系统提交取消到携程 [{1}]\n ", order.caozuo, DateTime.Now.ToString());
                    order.remark = string.Format("{0}【携程】:订单取消失败，具体原因:{1}  [{2}]<br/>", order.remark, cancelResult.Message, DateTime.Now.ToString());
                    result.SetError("订单取消失败");
                }
                var rows = SqlSugarContext.BigTreeInstance.Updateable(order).ExecuteCommand();


                return result;
            }
            catch (Exception ex)
            {
                return result.SetError("订单取消失败，发生异常！");
            }
        }
        #endregion

        #region 创建订单
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult CreateOrder(int id)
        {
            ChenYiMeiTuanOrderChannel chenYiMeiTuanOrder = new ChenYiMeiTuanOrderChannel(Shop);
            var synResult = chenYiMeiTuanOrder.SynChannelOrder(id);
            if (synResult.IsSucess)
            {
                var order = orderRepository.GetOrderByAid(id);
                chenYiCtripOrderChannel.CreateOrder(order.taoBaoOrderId);
            }
            else
            {
                var order = orderRepository.GetOrderByAid(id);
                order.remark = string.Format("{0}【系统】推送CY失败，具体原因：{1}<br/>", order.remark, synResult.Message);
                SqlSugarContext.BigTreeInstance.Updateable(order).UpdateColumns(u => u.remark).ExecuteCommand();
            }
            return synResult;
        }
        #endregion
    }
}
