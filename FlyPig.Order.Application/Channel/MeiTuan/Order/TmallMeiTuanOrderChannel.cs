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
using FlyPig.Order.Application.Channel.MeiTuan.Entity.mtPriceCacheEntity;
using FlyPig.Order.Framework.Logging;
using Flypig.Order.Application.Order.Entities;
using Newtonsoft.Json;

namespace FlyPig.Order.Application.MT.Order
{
    public abstract class TmallMeiTuanOrderChannel : IOrderChannel
    {
        protected IOrderRepository orderRepository;
        private readonly SqlSugarClient sqlSugarClient;
        protected Random random = new Random(Guid.NewGuid().GetHashCode());
        private readonly LogWriter logWriter;
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
            logWriter = new LogWriter("Tmall/Breakfast");
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

                //List<DailyInfo> dailyInfo = JsonConvert.DeserializeObject<List<DailyInfo>>(order.DailyInfoPrice);

                int tBreakfast = getBreakfast(order.hotelID, order.roomID, order.ratePlanID, order.orderNO);
                //logWriter.Write("美团：酒店id：{0},roomID：{1},ratePlanID：{2}，早餐数：{3}，飞猪早餐数：{4}，订单号：{5}", order.hotelID, order.roomID, order.ratePlanID, tBreakfast,order.userID,order.orderNO);
                //判断早餐数是否对等
                if (order.userID > tBreakfast && tBreakfast != 99 && order.userID != 0)
                {

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            //当早餐数不对等时更新rp
                            string urlUpdateRp = string.Format("http://localhost:9092/ashx/rnxUpdateRaPlan.ashx?type=rp&source=5&hid={0}", order.hotelID);
                            string getBoby = WebHttpRequest.Get(urlUpdateRp);
                            if (!getBoby.Contains("更新价格计划成功"))
                            {
                                WebHttpRequest.Get(urlUpdateRp);
                            }
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
                    string beiZhu = string.Format("【系统】:下单失败，具体原因：早餐个数不相等，天猫：{1}, 供应商：{2}，取消执行自动下单，请联系申退 [{0}] <br/>【系统】:已自动更正早餐数，无需人工手动下线该酒店或房型 [{0}]", DateTime.Now.ToString(), order.userID, tBreakfast);
                    orderRepository.UpdateRemarkState(order.aId, 24, beiZhu);
                    logWriter.Write("美团(供应商早餐数发生变化)：飞猪酒店id：{0}，酒店id：{1},roomID：{2},ratePlanID：{3}，早餐数：{4}，飞猪早餐数：{5}，订单号：{6}",order.TaoBaoHotelId, order.hotelID, order.roomID, order.ratePlanID, tBreakfast, order.userID, order.orderNO);
                    return result.SetError("该订单早餐个数不相等");
                }
                else if (order.userID < tBreakfast)
                {
                    //如果供应商的早餐数大于平台的，更新rp
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            string urlUpdateRp = string.Format("http://localhost:9092/ashx/rnxUpdateRaPlan.ashx?type=rp&source=5&hid={0}", order.hotelID);
                            string getBoby = WebHttpRequest.Get(urlUpdateRp);
                            if (!getBoby.Contains("更新价格计划成功"))
                            {
                                WebHttpRequest.Get(urlUpdateRp);
                            }
                        }
                        catch
                        {

                        }
                    });
                    logWriter.Write("美团(平台早餐数少)：飞猪酒店id：{0}，酒店id：{1},roomID：{2},ratePlanID：{3}，早餐数：{4}，飞猪早餐数：{5}，订单号：{6}", order.TaoBaoHotelId, order.hotelID, order.roomID, order.ratePlanID, tBreakfast, order.userID, order.orderNO);

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
                    || content.Contains("订单价格与产品价格不符, 可能已变价") || content.Contains("失败，已变价，请判断价格再提交"))
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

        /// <summary>
        /// 获取早餐数
        /// </summary>
        /// <param name="hotelCode"></param>
        /// <param name="RoomTypeId"></param>
        /// <param name="RatePlanId"></param>
        /// <returns></returns>
        public int getBreakfast(string hotelCode,string RoomTypeId, string RatePlanId,string orderNO)
        {
            int Breakfast = 0;
            try
            {
                string sql = string.Format(@"SELECT top 1  cache as RoomName FROM dbo.mtPriceCache with(nolock) WHERE hotelCode='{0}'", hotelCode);
                var RoomNameobj = SqlSugarContext.CacheData2_99.SqlQueryable<CtripRoomType>(sql).First();
                if (RoomNameobj != null)
                {
                    if (!string.IsNullOrEmpty(RoomNameobj.RoomName))
                    {
                        var rts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<mtPriceCache>>(RoomNameobj.RoomName);
                        //var RoomTypeInfo = rts.Where(b => b.XRoomId == RoomTypeId && b.RatePlan[0].RatePlanId == RatePlanId).FirstOrDefault();
                        var RoomTypeInfo = rts.Where(b => b.RatePlan[0].RatePlanId == RatePlanId).FirstOrDefault();
                        //var RoomTypeInfo = rp.RatePlan.Where(b => b.RatePlanId == RatePlanId&& b.RoomTypeId== RoomTypeId).FirstOrDefault();
                        Breakfast = Convert.ToInt16(RoomTypeInfo.RatePlan[0].Breakfast.Replace("份早餐", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                logWriter.Write("美团(ERR)：酒店id：{0},roomID：{1},ratePlanID：{2}，订单号：{3}，报错原因：{4}", hotelCode, RoomTypeId, RatePlanId, orderNO, ex.ToString());
                Breakfast = 99;
            }
            return Breakfast;
        }
    }
}
