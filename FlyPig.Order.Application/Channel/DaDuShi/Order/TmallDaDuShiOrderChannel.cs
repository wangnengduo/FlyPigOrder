using Flypig.Order.Application.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flypig.Order.Application.Common;
using FlyPig.Order.Application.Repository.Order;
using SqlSugar;
using FlyPig.Order.Framework.Logging;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Hotel.Channel;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using FlyPig.Order.Framework.HttpWeb;
using Flypig.Order.Application.Order.Message;
using DaDuShi.Entities;
using DaDuShi.Common;
using DaDuShi.Request;
using DaDuShi.Response;

namespace FlyPig.Order.Application.Channel.DaDuShi.Order
{
    public abstract class TmallDaDuShiOrderChannel : IOrderChannel
    {
        protected IOrderRepository orderRepository;
        private readonly SqlSugarClient sqlSugarClient;
        protected Random random = new Random(Guid.NewGuid().GetHashCode());
        private readonly LogWriter logWriter;
        private readonly LogWriter orderLogWriter;
        private readonly LogWriter cancellogWriter;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shop"></param>
        public TmallDaDuShiOrderChannel(ShopType shop)
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
            logWriter = new LogWriter("DDS/Breakfast");
            cancellogWriter = new LogWriter("DDS/CancelOrder");
            orderLogWriter = new LogWriter("DDS/Order/");
        }

        public abstract string RequestUrl { get; }

        protected ShopType Shop { get; set; }

        protected ProductChannel Channel { get { return ProductChannel.DDS; } }

        #region Web服务操作对象（订单）  OrderDataOpEt
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
        #endregion

        public abstract ServiceResult SynChannelOrder(int aid);

        public abstract bool UpdateCancelRemark(long taobaoOrderId);

        public ServiceResult CancelOrder(int id)
        {
            var result = new ServiceResult();
            var request = new OrderRequestJsonEt();
            var response = new WebServiceResponse<CancelOrderStatusJsonEt>();
            try
            {
                //  bool islimit = true;

                var order = sqlSugarClient.Queryable<TB_hotelcashorder>().Where(u => u.aId == id).First();
                if (order == null)
                {
                    return result.SetError("订单不存在");
                }
                if (string.IsNullOrEmpty(order.sourceOrderID))
                {
                    return result.SetError("未下单到大都市");
                }
                string message = string.Empty;
                request = new OrderRequestJsonEt
                {
                    ProductReservationId = order.sourceOrderID,
                };
                try
                {
                    response = OrderDataOpEt.Cancel(request);
                }
                catch (Exception ex)
                {
                    response = OrderDataOpEt.Cancel(request);
                }
                //记录取消日志
                try
                {
                    string orderText = string.Empty;
                    string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    orderText = "取消订单号：" + order.orderNO + "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse + "\r\n" + "------------------------------------------";
                    cancellogWriter.Write(orderText);
                }
                catch (Exception exOrder)
                {
                }
                string StatusName = string.Empty;
                if (response != null && response.ResponseEt != null && response.ResponseEt.Status != null)
                {
                    if (response.ResponseEt.Status.ToLower().Contains("successful"))
                    {
                        StatusName = "取消成功";
                        string remark = string.Format("【大都市】:订单取消成功 [{0}]", DateTime.Now.ToString());
                        orderRepository.UpdateRemarkState(order.aId, 3, remark);
                        //orderRepository.UpdateOrderRemark(order.aId, remark);
                    }
                    else if (response.ResponseEt.Status.ToLower().Contains("failed") || !response.Successful)
                    {
                        StatusName = "取消失败";
                        if (!response.Successful && !string.IsNullOrWhiteSpace(response.ErrMsg))
                        {
                            StatusName = StatusName + "。" + response.ErrMsg;
                        }
                        string remark = string.Format("【大都市】:订单取消失败，具体原因(取消失败，请人工跟进),{1} [{0}]", DateTime.Now.ToString(), response.ErrMsg);
                        orderRepository.UpdateRemarkState(order.aId, 7, remark);
                    }
                    else if (!response.Successful)
                    {
                        StatusName = "取消失败";
                        if (!response.Successful && !string.IsNullOrWhiteSpace(response.ErrMsg))
                        {
                            StatusName = StatusName + "。" + response.ErrMsg;
                        }
                        string remark = string.Format("【大都市】:订单取消失败，具体原因(取消失败，请人工跟进),{1} [{0}]", DateTime.Now.ToString(), response.ErrMsg);
                        orderRepository.UpdateRemarkState(order.aId, 7, remark);
                    }
                    else
                    {
                        StatusName = "未知状态";
                        string remark = string.Format("<br/>【大都市】:订单取消失败，具体原因(取消失败，请人工跟进) [{0}]", DateTime.Now.ToString());
                        orderRepository.UpdateRemarkState(order.aId, 7, remark);
                    }
                }
                else if (!response.Successful)
                {
                    StatusName = "取消失败";
                    if (!response.Successful && !string.IsNullOrWhiteSpace(response.ErrMsg))
                    {
                        StatusName = StatusName + "。" + response.ErrMsg;
                    }
                    if (!order.remark.Contains("该订单已被拒单"))
                    {
                        string remark = string.Format("【大都市】:订单取消失败，具体原因(取消失败，请人工跟进),{1} [{0}]", DateTime.Now.ToString(), response.ErrMsg);
                        orderRepository.UpdateRemarkState(order.aId, 7, remark);
                    }
                }
                message += string.Format("【系统】提交取消申请，返回结果：{0} [{1}]", StatusName, DateTime.Now.ToString());
                orderRepository.UpdateOrderRemark(order.aId, message);



                return result.SetSucess("提交取消申请成功");

            }
            catch (Exception ex)
            {
                string message = string.Format("【系统】提交取消申请，出现异常:{0}", ex.ToString());
                orderRepository.UpdateRemarkState(id,7, message);
                //记录取消日志
                try
                {
                    string orderText = string.Empty;
                    string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    orderText = "取消订单id号：" + id + "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse + "\r\n" + "------------------------------------------";
                    cancellogWriter.Write(orderText);
                }
                catch (Exception exOrder)
                {
                }
                return result.SetError("提交取消申请失败");
            }
        }

        public ServiceResult CreateOrder(int id)
        {
            var result = new ServiceResult();
            var RequestEt = new CreateOrderJsonEt();
            var response = new WebServiceResponse<OrderInfoJsonEt>();
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
                    return result.SetError("该订单未提交到大都市，请勿重复下单");
                }
                if (order.totalPrice == 0)
                {
                    string remark = string.Format("【系统】:下单到大都市失败（已售完或扣减库存失败） [{0}]", DateTime.Now.ToString());
                    orderRepository.UpdateRemarkState(order.aId, 63, remark);

                    int NowHour = DateTime.Now.Hour;//当前时间的时数
                    int NowMinute = DateTime.Now.Minute;//当前时间的分钟数
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            //晚上12点半后，8点前满房时自动发送短信
                            if ((NowHour == 0 && NowMinute > 30) || (NowHour > 0 && NowHour < 8))
                            {
                                //发满房短信（发送满房信息）
                                FlyPigMessageUility.ThirdCommunication(Shop, Convert.ToInt32(order.aId), "系统", 1);
                            }
                            AliTripValidate av = new AliTripValidate();
                            av.CheckInTime = order.checkInDate;
                            av.CheckOutTime = order.checkOutDate;
                            av.HotelId = order.hotelID;
                            av.RoomId = order.roomID;
                            av.RatePlanId = order.ratePlanID;
                            av.RatePlanCode = "dr" + order.hotelID + "_" + order.roomID + "_" + order.ratePlanID;
                            av.CreateTime = DateTime.Now;
                            av.IsFull = true;
                            av.Channel = (int)Channel;
                            av.Shop = (int)Shop;
                            av.Remark = "下单满房";
                            SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();

                            string updateUrl = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source={1}", order.hotelID, (int)Channel);
                            WebHttpRequest.Get(updateUrl);
                        }
                        catch
                        {

                        }
                    });
                    return result.SetError("订单底价为0，无法下单到渠道！");
                }

                //判断早餐数是否对等
                if (order.hasChange == 1 && order.userID != 0 && order.userID > order.sRoomNum)
                {
                    string beiZhu = string.Format("【系统】:下单失败，具体原因：早餐个数不相等，天猫：{1}, 供应商：{2}，取消执行自动下单，请联系申退 [{0}] <br/>【系统】:已自动更正早餐数，无需人工手动下线该酒店或房型 [{0}]", DateTime.Now.ToString(), order.userID, order.sRoomNum);
                    //string beiZhu = string.Format("【系统】:下单失败，具体原因：早餐个数不相等！已自动更正早餐数，无需人工手动下线该酒店或房型 [{0}]", DateTime.Now.ToString());
                    orderRepository.UpdateRemarkState(order.aId, 24, beiZhu);
                    int NowHour = DateTime.Now.Hour;//当前时间的时数
                    int NowMinute = DateTime.Now.Minute;//当前时间的分钟数
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            //晚上12点半后，8点前满房时自动发送短信
                            if ((NowHour == 0 && NowMinute > 30) || (NowHour > 0 && NowHour < 8))
                            {
                                //发满房短信（发送满房信息）
                                FlyPigMessageUility.ThirdCommunication(Shop, Convert.ToInt32(order.aId), "系统", 1);
                            }
                            AliTripValidate av = new AliTripValidate();
                            av.CheckInTime = order.checkInDate;
                            av.CheckOutTime = order.checkOutDate;
                            av.HotelId = order.hotelID;
                            av.RoomId = order.roomID;
                            av.RatePlanId = order.ratePlanID;
                            av.RatePlanCode = "dr" + order.hotelID + "_" + order.roomID + "_" + order.ratePlanID;
                            av.CreateTime = DateTime.Now;
                            av.IsFull = true;
                            av.Channel = (int)Channel;
                            av.Shop = (int)Shop;
                            av.Remark = "下单满房";
                            SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();

                            string updateUrl = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source={1}", order.hotelID, (int)Channel);
                            WebHttpRequest.Get(updateUrl);
                        }
                        catch
                        {

                        }
                    });
                    logWriter.Write("大都市(供应商早餐数发生变化)：飞猪酒店id：{0}，酒店id：{1},roomID：{2},ratePlanID：{3}，早餐数：{4}，飞猪早餐数：{5}，订单号：{6}", order.TaoBaoHotelId, order.hotelID, order.roomID, order.ratePlanID, order.sRoomNum, order.userID, order.orderNO);
                    return result.SetError("该订单早餐个数不相等");
                }

                #region 下订单

                ContactPersonJsonEt contactPerson = new ContactPersonJsonEt();
                contactPerson.GivenName = order.contactName;
                contactPerson.Surname = order.contactName;
                contactPerson.Telephone = order.contactTel;
                contactPerson.Email = "";
                contactPerson.Address = "";

                List<Guest> guestList = new List<Guest>();
                List<string> guestNames = order.guestName.Split(',').ToList();
                foreach (var item in guestNames)
                {
                    Guest guest = new Guest();
                    guest.GivenName = item;
                    guest.Surname = item;
                    guest.Telephone = "";
                    guest.Email = "";
                    guestList.Add(guest);
                }


                GuestCount guestCount = new GuestCount();
                guestCount.AdultCount = guestNames.Count;
                guestCount.ChildCount = 0;

                RequestEt.HotelCode = order.hotelID;
                RequestEt.RoomTypeCode = order.roomID;
                RequestEt.RatePlanCode = order.ratePlanID;
                RequestEt.NumberOfUnits = order.roomNum;
                RequestEt.DistributorReservationId = order.orderNO;
                RequestEt.TotalAmount = order.totalPrice;
                RequestEt.CheckIn = order.checkInDate.ToString("yyyy-MM-dd");
                RequestEt.CheckOut = order.checkOutDate.ToString("yyyy-MM-dd");
                RequestEt.Comment = "";
                RequestEt.ContactPerson = contactPerson;
                RequestEt.GuestList = guestList;
                RequestEt.GuestCount = guestCount;

                response = OrderDataOpEt.Reservation(RequestEt);
                //记录预订日志
                try
                {
                    string orderText = string.Empty;
                    string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(RequestEt);
                    string strresponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    orderText = "请求数据：" + strRequest + "\r\n" + "返回数据：" + strresponse;
                    orderLogWriter.WriteOrder(order.orderNO, orderText);
                }
                catch (Exception exOrder)
                {
                }

                var createResult = new ServiceResult();

                string message = string.Empty;
                if (response != null && response.Successful && response.ResponseEt != null)
                {
                    string ProductReservationId = response.ResponseEt.ProductReservationId.ToString();//大都市产品系统订单编号
                    string ErpReservationId = response.ResponseEt.ErpReservationId.ToString();//大都市结算系统订单编号
                    order.sourceOrderID = ProductReservationId;
                    message = string.Format("【大都市】:下单成功,订单号：({0})，大都市结算系统订单编号：{1} [{2}]", ProductReservationId, ErpReservationId, DateTime.Now.ToString());
                    //Pending: 待确认, Confirmed:已确认, Canceled:已取消, Failed：发生错误


                    //order.remark = string.Format("{0}【大都市】:{1} [{2}]<br/>", order.remark, message, DateTime.Now.ToString());
                    orderRepository.UpdateOrderRemarkAndSourceOrderID(order.aId,3, message, ProductReservationId);

                    return result.SetSucess(message);
                }
                else if (response != null && !response.Successful && !string.IsNullOrWhiteSpace(response.ErrMsg))
                {
                    message = string.Format("【系统】:提交创建大都市订单请求失败,失败原因：{1} [{0}]", DateTime.Now.ToString(),response.ErrMsg);
                    orderRepository.UpdateRemarkState(order.aId,7, message);
                    return result.SetError("下单失败，出现异常，错误原因{0}", response.ErrMsg);
                }
                else
                {
                    message = string.Format("【系统】:提交创建大都市订单请求失败，请查看渠道状态是否生成订单 [{0}]", DateTime.Now.ToString());
                    orderRepository.UpdateRemarkState(order.aId,7, message);
                    return result.SetError("下单失败，出现异常，请查看渠道状态是否生成订单");
                }
                #endregion

                return result.SetSucess(message);
            }
            catch (Exception ex)
            {
                string message = string.Format("<br/>【系统】下单失败，出现异常，请手动下单,异常内容：{0}", ex.ToString());
                orderRepository.UpdateRemarkState(id,7, message);
                return result.SetError("下单失败，出现异常，请手动下单");
            }
        }

    }
}
