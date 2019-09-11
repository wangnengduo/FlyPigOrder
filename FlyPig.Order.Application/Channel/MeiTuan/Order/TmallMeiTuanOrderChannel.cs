using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Common;
using FlyPig.Order.Application.Hotel;
using FlyPig.Order.Application.Hotel.Channel;
using FlyPig.Order.Application.MT.Order.Channel;
using FlyPig.Order.Application.Repository.Order;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using FlyPig.Order.Framework.HttpWeb;

using SqlSugar;
using Flypig.Order.Application.Order.Message;

namespace FlyPig.Order.Application.MT.Order
{
    public abstract class TmallMeiTuanOrderChannel : IOrderChannel
    {
        protected IOrderRepository orderRepository;
        private readonly SqlSugarClient sqlSugarClient;
        protected Random random = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shop"></param>
        public TmallMeiTuanOrderChannel(ShopType shop)
        {
            Shop = shop;
            orderRepository = ProductChannelFactory.CreateOrderRepository(shop, Channel);

            if (shop == ShopType.ShengLv || Shop == ShopType.RenNiXing)
            {
                sqlSugarClient = SqlSugarContext.ResellbaseInstance;
            }
            else
            {
                sqlSugarClient = SqlSugarContext.ResellbaseInstance;
            }
        }

        public abstract string RequestUrl { get; }

        protected ShopType Shop { get; set; }



        protected ProductChannel Channel { get { return ProductChannel.MT; } }


        public abstract ServiceResult SynChannelOrder(int aid);

        public abstract bool UpdateCancelRemark(long taobaoOrderId);

        public ServiceResult CancelOrder(int id)
        {
            var result = new ServiceResult();
            try
            {
                //  bool islimit = true;

                var order = sqlSugarClient.Queryable<TB_hotelcashorder>().Where(u => u.aId == id).First();
                if (order == null)
                {
                    return result.SetError("订单不存在");
                }
                string message = string.Empty;

                string url = string.Empty;
                if (Shop == ShopType.LingZhong || Shop == ShopType.YinJi || Shop == ShopType.RenXing)
                {
                    url = string.Format("http://47.107.30.243:9000/xw/orderCancel?saleorderno={0}&reason=", order.taoBaoOrderId);
                }
                else
                {
                    if (Shop == ShopType.RenNiXing)
                    {
                        url = string.Format("http://localhost:8094/rnx/orderCancel?saleorderno={0}&reason=&salesource=32", order.taoBaoOrderId);
                    }
                    else
                    {
                        url = string.Format("http://47.96.68.32/mtservice/orderCancel.ashx?saleorderno={0}&reason=&salesource=4", order.taoBaoOrderId);
                    }
                }

                string content = WebHttpRequest.GetOrder(url);
                message += string.Format("<br/>【系统】提交取消申请，返回结果：{0} [{1}]", content, DateTime.Now.ToString());
                orderRepository.UpdateOrderRemark(order.aId, message);

                if (Shop != ShopType.ShengLv)
                {
                    UpdateCancelRemark(order.taoBaoOrderId);
                }


                return result.SetSucess("提交取消申请成功");

            }
            catch (Exception ex)
            {
                string message = string.Format("【系统】提交取消申请，出现异常:{0}", ex.ToString());
                orderRepository.UpdateOrderRemark(id, message);
                return result.SetError("提交取消申请失败");
            }
        }

        public ServiceResult CreateOrder(int id)
        {
            var result = new ServiceResult();
            try
            {
                var order = sqlSugarClient.Queryable<TB_hotelcashorder>().Where(u => u.aId == id).First();
                if (order == null)
                {
                    return result.SetError("订单不存在");
                }

                if (order.alipayPay == 0)
                {
                    return result.SetError("该订单未付款");
                }

                if (order.state != 24)
                {
                    return result.SetError("该订单状态已变更");
                }

                if (order.Refuse == 2)
                {
                    return result.SetError("该订单已取消");
                }

                if (!string.IsNullOrEmpty(order.sourceOrderID))
                {
                    return result.SetError("该订单未提交到美团，请勿重复下单");
                }

                string message = string.Empty;
                string content = string.Empty;
                string url = string.Empty;
                if (Shop == ShopType.LingZhong || Shop == ShopType.YinJi || Shop == ShopType.RenXing)
                {
                    var synResult = SynChannelOrder((int)order.aId);
                    if (synResult.IsSucess)
                    {
                        content = result.Message;
                        message = string.Format("【系统】:同步订单请求成功，结果：{0} [{1}]", content, DateTime.Now.ToString());

                        if (order.orderType == 11)
                        {
                            url = string.Format("http://47.96.68.32/mtservice/orderCreate.ashx?saleorderno={0}&salesource=4", order.taoBaoOrderId, RequestUrl);
                        }
                        else
                        {
                            url = string.Format("http://47.107.30.243:9000/xw/orderCreate?saleorderno={0}", order.taoBaoOrderId);
                        }
                    }
                }
                else
                {
                    if (Shop == ShopType.RenNiXing)
                    {
                        url = string.Format("http://localhost:8094/rnx/orderCreate?saleorderno={0}&salesource=32", order.taoBaoOrderId);
                    }
                    else
                    {
                        url = string.Format("http://47.96.68.32/mtservice/orderCreate.ashx?saleorderno={0}&salesource=4", order.taoBaoOrderId, RequestUrl);
                    }
                }

                content = WebHttpRequest.GetOrder(url);

                if (content.Contains("可预订接口返回不可订") || content.Contains("订单提交给美团失败") || content.Contains("已售完或扣减库存失败")
                    || content.Contains("订单价格与产品价格不符, 可能已变价"))
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            AliTripValidate av = new AliTripValidate();
                            av.CheckInTime = order.checkInDate;
                            av.CheckOutTime = order.checkOutDate;
                            av.HotelId = order.hotelID;
                            av.RoomId = order.roomID;
                            av.RatePlanId = order.ratePlanID;
                            av.RatePlanCode = "mr" + order.hotelID + "_" + order.roomID + "_" + order.ratePlanID;
                            av.CreateTime = DateTime.Now;
                            av.IsFull = true;
                            av.Channel = (int)Channel;
                            av.Shop = (int)Shop;
                            av.Remark = "下单满房";
                            SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();

                            string updateUrl = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source={1}", order.hotelID, (int)Channel);
                            WebHttpRequest.Get(updateUrl);

                            orderRepository.UpdateOrderStutas(order.aId, 63);

                            int NowHour = DateTime.Now.Hour;//当前时间的时数
                            int NowMinute = DateTime.Now.Minute;//当前时间的分钟数
                            //晚上12点半后，8点前满房时自动发送短信
                            if ((NowHour == 0 && NowMinute > 30) || (NowHour > 0 && NowHour < 8))
                            {
                                //发满房短信（发送满房信息）
                                FlyPigMessageUility.ThirdCommunication(Shop, Convert.ToInt32(order.aId), "系统", 1);
                            }
                        }
                        catch
                        {

                        }
                    });
                    //string caozuo = string.Format("<br/>【系统】已发送满房短信[{0}]", DateTime.Now.ToString());
                    ////发满房短信
                    //FlyPigMessageUility.ThirdCommunication(Shop,Convert.ToInt32(order.aId), "系统", 1);
                    ////更新状态和操作
                    //orderRepository.UpdateRemarkStateCaoZuo(order.aId, 63,"", caozuo);
                }
                message = string.Format("<br/>【系统】:提交创建美团订单请求成功，结果：{0} [{1}]", content, DateTime.Now.ToString());
                orderRepository.UpdateOrderRemark(order.aId, message);
                return result.SetSucess(content);
            }
            catch (Exception ex)
            {
                string message = string.Format("<br/>【系统】下单失败，出现异常，请手动下单,异常内容：{0}", ex.ToString());
                orderRepository.UpdateOrderRemark(id, message);
                return result.SetError("下单失败，出现异常，请手动下单");
            }
        }

    }
}
