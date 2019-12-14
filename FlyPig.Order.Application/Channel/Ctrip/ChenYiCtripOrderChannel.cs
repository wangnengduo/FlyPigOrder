using Ctrip;
using Ctrip.Config;
using Ctrip.Request;
using CtripApi.NewRequest;
using Flypig.Order.Application.Common;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Repository.Order;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using FlyPig.Order.Framework.HttpWeb;
using FlyPig.Order.Framework.Logging;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LingZhong.HotelManager.Application.Channel.Ctrip.Order
{
    public class ChenYiCtripOrderChannel
    {
        private readonly LogWriter orderLogWriter;
        private readonly LogWriter noticeLogWriter;
        private ThirdOrderRepository orderRepository;
        private readonly Random random = new Random();
        private readonly ChenYiOrderRepository chenYiOrderRepository;

        public ChenYiCtripOrderChannel(ShopType shop)
        {
            if (shop == ShopType.ShengLv || shop == ShopType.RenNiXing)
            {
                orderRepository = new ShengLvOrderRepository();
            }
            else
            {
                orderRepository = new ThirdOrderRepository();
            }

            Shop = shop;
            chenYiOrderRepository = new ChenYiOrderRepository();
            noticeLogWriter = new LogWriter("Ctrip/Notice/");
            orderLogWriter = new LogWriter("Ctrip/Order/");
        }

        public ShopType Shop { get; set; }

        public ProductChannel Channel
        {
            get
            {
                return ProductChannel.Ctrip;
            }
        }

        private CtripApiClient GetApiClient(int priceOrderWay)
        {
            CtripApiClient ctripApi = null;
            if (priceOrderWay == 66)
            {
                ctripApi = new CtripApiClient(ApiConfigManager.ChenYi);
            }
            else if (priceOrderWay == 16)
            {
                ctripApi = new CtripApiClient(ApiConfigManager.LingNan);
            }
            else if (priceOrderWay == 86)
            {
                ctripApi = new CtripApiClient(ApiConfigManager.ZhiHeC);
            }
            return ctripApi;
        }

        #region 下单
        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult CreateOrder(long id)
        {
            var result = new ServiceResult();

            var cOrder = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => (u.PriceOrderWay == 66 || u.PriceOrderWay == 16 || u.PriceOrderWay == 86) && u.fax == id.ToString()).First();
            if (cOrder != null && !string.IsNullOrEmpty(cOrder.ElongOrderID))
            {
                return result.SetError("已下单到携程，请勿重新下单");
            }

            if (cOrder.pingtai == "ay" || cOrder.pingtai == "rnx")
            {
                orderRepository = new ShengLvOrderRepository();
            }
            else
            {
                orderRepository = new ThirdOrderRepository();
            }

            var order = orderRepository.GetOrderByTaoBaoOrderId(id);
            SqlSugarClient sugarClient = null;

            string faildOrderKey = string.Empty;
            if (order.shopType == 7 || order.shopType == 9)
            {
                sugarClient = SqlSugarContext.ResellbaseInstance;
            }
            else
            {
                sugarClient = SqlSugarContext.BigTreeInstance;
            }

            if ((!string.IsNullOrEmpty(cOrder.beizhu) && cOrder.beizhu.Contains("订单申请取消")) || order.Refuse == 2 || order.remark.Contains("淘宝申请退款") || order.remark.Contains("淘宝执行订单取消"))
            {
                return result.SetError("分销已取消，取消下单");
            }

            var zOrder = new XZorder
            {
                comeDate = order.checkInDate,
                contactName = order.contactName,
                contactPhone = "18022416577",
                guestNames = order.guestName.Split(',').ToList(),
                guestNum = order.guestName.Split(',').Count(),
                hotelCode = order.hotelID,
                lateArriveTime = order.lastArriveTime,
                leaveDate = order.checkOutDate,
                orderNo = order.sourceOrderID,
                //price = Convert.ToDecimal(order.totalPrice),//指导价
                salePrice = Convert.ToDecimal(order.alipayPay),//卖价
                totalPrice = Convert.ToDecimal(order.totalPrice),//底价
                ratePlanCategory = "501",
                ratePlanCode = order.ratePlanID,
                roomNum = order.roomNum,
                specialRequest = order.comment,
            };

            var createResult = new ServiceResult();
            if (cOrder.PriceOrderWay == 86)
            {
                createResult = CreatePromotionOrder(zOrder);
            }
            else
            {
                createResult = CreateNormalOrder((int)cOrder.PriceOrderWay, zOrder);
            }


            if (cOrder != null)
            {
                cOrder.caozuo = string.Format("{0}system 操作下单到携程 [{1}]\n", cOrder.caozuo, DateTime.Now.ToString());
            }

            if (createResult.IsSucess)
            {
                string ctripOrderId = createResult.Data.ToString();
                cOrder.ElongOrderID = ctripOrderId;
                string message = string.Format("下单成功,订单号：({0})", ctripOrderId);
                cOrder.state = 3;

                if (string.IsNullOrEmpty(cOrder.beizhu))
                {
                    cOrder.beizhu = string.Format("【系统】:{0} [{1}]<br/>", message, DateTime.Now.ToString());
                }
                else
                {
                    cOrder.beizhu = string.Format("{2}【系统】:{0} [{1}]<br/>", message, DateTime.Now.ToString(), cOrder.beizhu);
                }

                var rows = SqlSugarContext.TravelskyInstance.Updateable(cOrder).ExecuteCommand();
                if (rows > 0)
                {
                    var payResult = PayOrder(id);  //下单后支付
                }

                order.remark = string.Format("{0}【携程】:{1} [{2}]<br/>", order.remark, message, DateTime.Now.ToString());
                rows = sugarClient.Updateable(order).UpdateColumns(u => u.remark).ExecuteCommand();
                
                return result.SetSucess(message);
            }
            else
            {
                cOrder.state = 1;
                cOrder.beizhu = string.Format("{0}【系统】:下单失败，{1}  [{2}]<br/>", cOrder.beizhu, createResult.Message, DateTime.Now.ToString());

                var rows = SqlSugarContext.TravelskyInstance.Updateable(cOrder).ExecuteCommand();
                
                return result.SetError(createResult.Message);
            }
        }

        /// <summary>
        /// 下订单
        /// </summary>
        /// <param name="priceOrderWay"></param>
        /// <param name="zOrder"></param>
        /// <returns></returns>
        public ServiceResult CreateNormalOrder(int priceOrderWay, XZorder zOrder)
        {
            var result = new ServiceResult();

            var ctripApiClient = GetApiClient(priceOrderWay);
            var response = ctripApiClient.SubmitOrderV(zOrder);
            if (response.code == "0")
            {
                result.SetData(response.result.orderId);
                return result.SetSucess("下单到携程成功");
            }
            else
            {
                return result.SetError(response.errMsg);
            }
        }

        /// <summary>
        /// 促销下单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ServiceResult CreatePromotionOrder(XZorder order)
        {
            var result = new ServiceResult();

            var request = new CtripApi.NewRequest.CreateOrderRequest { };
            var apiConfig = ApiConfigManager.ZhiHeC;
            request.UniqueID = new List<UniqueIDItem> {
               //new UniqueIDItem{ ID="28", Type=apiConfig.AllianceID },
               //new UniqueIDItem{ ID="503", Type=apiConfig.SID },
                new UniqueIDItem{ ID=apiConfig.UID, Type="1" },
                new UniqueIDItem{ ID=order.orderNo,Type="504"  },
            };

            List<PersonNameItem> personNames = new List<PersonNameItem>();
            foreach (var item in order.guestNames)
            {
                PersonNameItem name = new PersonNameItem() { Name = item };
                personNames.Add(name);
            }

            request.Version = "1.0";
            request.PrimaryLangID = "zh-cn";
            request.HotelReservations = new CtripApi.NewRequest.HotelReservations
            {
                HotelReservation = new CtripApi.NewRequest.HotelReservation
                {
                    ResGlobalInfo = new CtripApi.NewRequest.ResGlobalInfo
                    {
                        Total = new CtripApi.NewRequest.Total
                        {
                            InclusiveAmount = order.totalPrice,
                            ExclusiveAmount = 0,
                            CurrencyCode = "CNY"
                        },
                        TPA_Extensions = new TPA_Extensions1
                        {
                            TotalCost = new TotalCost
                            {
                                CurrencyCode = "CNY",
                                InclusiveAmount = order.salePrice,
                                ExclusiveAmount = 0,
                            }

                        },
                        GuestCounts = new CtripApi.NewRequest.GuestCounts
                        {
                            GuestCount = new CtripApi.NewRequest.GuestCount
                            {
                                Count = order.guestNames.Count
                            }
                        },
                        TimeSpan = new TimeSpanInfo
                        {
                            Start = order.comeDate.ToString("yyyy-MM-dd"),
                            End = order.leaveDate.ToString("yyyy-MM-dd")
                        },
                        DepositPayments = new List<string>
                        {

                        }
                    }
                    ,
                    RoomStays = new CtripApi.NewRequest.RoomStays
                    {
                        RoomStay = new CtripApi.NewRequest.RoomStay
                        {
                            RoomTypes = new CtripApi.NewRequest.RoomTypes
                            {
                                RoomType = new RoomType
                                {
                                    NumberOfUnits = order.roomNum
                                }
                            },
                            RatePlans = new CtripApi.NewRequest.RatePlans
                            {
                                RatePlan = new RatePlan
                                {
                                    RoomID = order.ratePlanCode,
                                    PrepaidIndicator = 1
                                }
                            },
                            BasicPropertyInfo = new BasicPropertyInfo
                            {
                                HotelCode = order.hotelCode
                            }
                        }
                    },
                    ResGuests = new CtripApi.NewRequest.ResGuests
                    {
                        ResGuest = new CtripApi.NewRequest.ResGuest
                        {
                            ArrivalTime = "23:30", //最早到店时间
                            TPA_Extensions = new TPA_Extensions
                            {
                                LateArrivalTime = "23:30"  //最晚到店时间
                            },
                            Profiles = new CtripApi.NewRequest.Profiles
                            {
                                ProfileInfo = new CtripApi.NewRequest.ProfileInfo
                                {
                                    Profile = new CtripApi.NewRequest.Profile
                                    {
                                        Customer = new CtripApi.NewRequest.Customer
                                        {
                                            ContactPerson = new CtripApi.NewRequest.ContactPerson
                                            {
                                                PersonName = new CtripApi.NewRequest.PersonName
                                                {
                                                    Name = order.contactName
                                                },
                                                Telephone = new Telephone
                                                {
                                                    PhoneNumber = order.contactPhone,
                                                    PhoneTechType = "5",
                                                }
                                                ,
                                                ContactType = "non"
                                            },
                                            PersonName = personNames
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var response = CtripNewClient.Excute(request);
            if (response.Errors.Count == 0)
            {
                var resInfo = response.HotelReservations.HotelReservation.ResGlobalInfo.HotelReservationIDs.Where(u => u.ResID_Type == "501").FirstOrDefault();
                result.SetData(resInfo.ResID_Value);
                return result.SetSucess("下单成功");
            }
            else
            {
                return result.SetError(response.Errors[0].Value);
            }
        }
        
        #endregion

        #region 支付订单
        /// <summary>
        /// 支付订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult PayOrder(long id)
        {
            var result = new ServiceResult();
            var order = chenYiOrderRepository.GetOrderByTaoBaoOrderId(id);
            if (order == null)
            {
                return result.SetError("订单不存在！");
            }

            string message = string.Empty;
            string remark = string.Empty;
            var totalPrice = Convert.ToDecimal(order.SpecialMemo) * order.num;//底价
            var salePrice = Convert.ToDecimal(order.ly_pingtai.Replace(":money:", "*").Split('*')[1]);//客户支付价

            //var isSuccess = ctripClient.SubmitOrderPayment(order.ElongOrderID, order.dingdan_num, totalPrice, out message);
            //var isSuccess = ctripClient.SubmitOrderPayment(order.ElongOrderID, order.dingdan_num, totalPrice, salePrice, out message);
            var submitResult = new ServiceResult();

            if (order.PriceOrderWay == 86)
            {
                submitResult = PayPromotionOrder(order.ElongOrderID, order.dingdan_num, totalPrice, salePrice);
            }
            else
            {
                submitResult = PayNormalOrder((int)order.PriceOrderWay, order.ElongOrderID, order.dingdan_num, totalPrice, salePrice);
            }

            if (submitResult.IsSucess)
            {
                order.state = 3;
                order.beizhu = string.Format("{0}【携程】:支付订单成功   [{1}]<br/>", order.beizhu, DateTime.Now.ToString());
                result.SetSucess("支付订单成功！");
            }
            else
            {
                order.state = 7;
                order.beizhu = string.Format("{0}【携程】:支付失败，具体原因：{1}   [{2}]<br/>", order.beizhu, submitResult.Message, DateTime.Now.ToString());
                result.SetError("支付订单失败，具体原因:{0}", submitResult.Message);
            }

            order.caozuo = $"{order.caozuo} system 调用支付接口 [{DateTime.Now.ToString()}] \n";
            var flag = SqlSugarContext.TravelskyInstance.Updateable(order).UpdateColumns(u => new { u.beizhu, u.state, u.caozuo }).ExecuteCommand() > 0;
            
            return result;
        }

        /// <summary>
        /// 支付订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult PayNormalOrder(int priceOrderWay, string ctripOrderId, string xwOrderId, decimal totalPrice, decimal salePrice)
        {
            var result = new ServiceResult();
            string message = string.Empty;
            var ctripClient = GetApiClient(priceOrderWay);
            var isSuccess = ctripClient.SubmitOrderPayment(ctripOrderId, xwOrderId, totalPrice, salePrice, out message);
            if (isSuccess)
            {
                result.SetSucess("支付订单成功！");
            }
            else
            {
                result.SetError($"{message}");
            }

            return result;
        }


        /// <summary>
        /// 支付订单（促销）
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ServiceResult PayPromotionOrder(string ctripOrderId, string xwOrderId, decimal totalPrice, decimal salePrice)
        {
            var result = new ServiceResult();
            string url = string.Format("http://47.106.65.149:8057/Pay/Zpay?orderId={0}&orderNo={1}&cost={2}&price={3}", ctripOrderId, xwOrderId, totalPrice, salePrice);

            try
            {
                var content = HttpUtility.GetHtml(url);
                if (!string.IsNullOrEmpty(content))
                {
                    var submitResult = JsonConvert.DeserializeObject<KResult>(content);
                    if (submitResult.suc)
                    {
                        return result.SetSucess("支付成功");
                    }
                    else
                    {
                        return result.SetError($"{submitResult.message}({submitResult.code})");
                    }
                }

                return result.SetError($"请求接口无任何响应");
            }
            catch (Exception ex)
            {
                return result.SetError($"出现异常,{ex.ToString()}");
            }
        }
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult CancelOrder(long id)
        {
            var result = new ServiceResult();
            string message = string.Empty;
            var order = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => (u.PriceOrderWay == 66 || u.PriceOrderWay == 16 || u.PriceOrderWay == 86) && u.fax == id.ToString()).First();
            var orderTB = SqlSugarContext.ResellbaseInstance.Queryable<TB_hotelcashorder>().Where(u => u.taoBaoOrderId == id).First();

            try
            {

                if (new List<int> { 2, 22 }.Contains(order.state))
                {
                    return result.SetSucess("取消成功，CY已取消！");
                }

                if (string.IsNullOrEmpty(order.ElongOrderID))
                {
                    return result.SetError("取消失败，未下单到携程！");
                }
                var ctripClient = GetApiClient((int)order.PriceOrderWay);
                var orderDetail = ctripClient.GetOrderDetail(order.ElongOrderID);
                if (orderDetail.OrderTags != null && orderDetail.OrderTags.Count > 0)
                {
                    var cancelCode = orderDetail.OrderTags.Where(u => u.Code == "AllowCancel").FirstOrDefault();
                    if (cancelCode != null && cancelCode.Value == "F" && orderDetail.OrderStatus != "Confirm")
                    {
                        try
                        {
                            order.state = 7;
                            order.beizhu = string.Format("{0}<span style='color:blue'>【携程】:订单取消失败，具体原因(取消失败，请人工跟进) [{1}] </span> <br/>", order.beizhu, DateTime.Now.ToString());
                            orderTB.state = 7;
                            orderTB.remark = string.Format("{0}<span style='color:blue'>【携程】:订单取消失败，具体原因(取消失败，请人工跟进) [{1}] </span> <br/>", orderTB.remark, DateTime.Now.ToString());
                            result.SetError("订单取消失败，具体原因(取消失败，请人工跟进)");
                            var updateSuccess = SqlSugarContext.TravelskyInstance.Updateable(order).UpdateColumns(u => new { u.beizhu, u.state }).ExecuteCommand() > 0;
                            var updateSuccessTb = SqlSugarContext.ResellbaseInstance.Updateable(orderTB).UpdateColumns(u => new { u.remark, u.state }).ExecuteCommand() > 0;
                            return result;
                        }
                        catch
                        {

                        }
                    }
                }

                if (order == null)
                {
                    return result.SetError("取消失败，订单不存在！");
                }

                string remark = string.Format("{0}【AY】：订单申请取消..    [{1}] <br/>", order.beizhu, DateTime.Now.ToString());
                //string remarkTB = string.Format("{0}【系统】：订单申请取消..    [{1}] <br/>", orderTB.remark, DateTime.Now.ToString());
                var isSussess = ctripClient.CancelOrder(order.ElongOrderID, ref message);
                if (isSussess)
                {
                    order.state = 2;
                    if (remark.Contains("该订单已确认"))
                    {
                        remark = string.Format("{0}【AY】：客人已撤销担保  [{1}]<br/>", remark, DateTime.Now.ToString());
                        order.state = 7;
                    }

                    order.beizhu = string.Format("{0}【携程】:订单取消成功 [{1}]<br/>", remark, DateTime.Now.ToString());

                    orderTB.remark = string.Format("{0}【携程】:订单取消成功 [{1}]<br/>", orderTB.remark, DateTime.Now.ToString());
                    result.SetSucess("订单取消成功");
                }
                else
                {
                    order.state = 7;
                    order.beizhu = string.Format("{0}【携程】:订单取消失败，具体原因({1}) [{2}]<br/>", remark, message, DateTime.Now.ToString());

                    orderTB.state = 7;
                    orderTB.remark = string.Format("{0}【携程】:订单取消失败，具体原因({1}) [{2}]<br/>", orderTB.remark, message, DateTime.Now.ToString());
                    result.SetError("订单取消失败，具体原因：{0}", message);
                }

                var flag = SqlSugarContext.TravelskyInstance.Updateable(order).UpdateColumns(u => new { u.beizhu, u.state }).ExecuteCommand() > 0;
                var flagTb = SqlSugarContext.ResellbaseInstance.Updateable(orderTB).UpdateColumns(u => new { u.remark, u.state }).ExecuteCommand() > 0;

            }
            catch
            {
            }

            return result;
        }
        #endregion

        public void GetOrderStatus(long taobaoOrderId)
        {
            string orderStatus = string.Empty;
            var order = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.fax == taobaoOrderId.ToString()).First();
            if (order == null)
            {
                orderStatus = "订单不存在";
            }
            else
            {
                if (string.IsNullOrEmpty(order.ElongOrderID))
                {
                    orderStatus = "未下单到携程";
                }

                var ctripClient = GetApiClient((int)order.PriceOrderWay);
                var orderResult = ctripClient.GetMiniOrderInfo(order.ElongOrderID);
                if (orderResult == null || string.IsNullOrEmpty(orderResult.OrderStatus))
                {
                    orderStatus = "获取携程状态失败";
                }
                else
                {
                    orderStatus = orderResult.OrderStatus;
                }
            }
        }

        public string GetXcOrderStatus(string taobaoOrderId)
        {
            string orderStatus = string.Empty;
            try
            {
                var order = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.fax == taobaoOrderId).First();
                if (order == null)
                {
                    orderStatus = "订单不存在";
                }
                else
                {
                    if (string.IsNullOrEmpty(order.ElongOrderID))
                    {
                        orderStatus = "未下单到携程";
                    }

                    var ctripClient = GetApiClient((int)order.PriceOrderWay);
                    var orderResult = ctripClient.GetMiniOrderInfo(order.ElongOrderID);
                    if (orderResult == null || string.IsNullOrEmpty(orderResult.OrderStatus))
                    {
                        orderStatus = "获取携程状态失败";
                    }
                    else
                    {
                        orderStatus = orderResult.OrderStatus;
                    }
                }
            }
            catch (Exception ex)
            {
                orderStatus = "查询失败";
            }
            return orderStatus;
        }


        #region 支付通知回调
        /// <summary>
        /// 支付通知回调
        /// </summary>
        /// <param name="orderType">订单类型；酒店固定为1</param>
        /// <param name="orderId">分销商订单号</param>
        /// <returns></returns>
        public string PayNotice(string orderType, string orderId)
        {
            string is_success = true.ToString().ToLower();

            decimal pay_amount = 0;
            string pay_currency = "CNY";
            string remark = "";
            string err_code = "0";  // -1 失败
            string err_msg = "支付成功";
            string source = string.Empty;
            string orderNum = string.Empty;
            string resultJson = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(orderType) || string.IsNullOrEmpty(orderId))
                {
                    noticeLogWriter.WriteOrder(orderId, "支付回调：携程回调参数有误！");
                    resultJson = JsonConvert.SerializeObject(new { is_success = false.ToString().ToLower(), pay_amount = 0, pay_currency = "", remark = "", err_code = "-1", err_msg = "参数有误" });
                    return resultJson;
                }

                orderId = orderId.Trim();
                var dingdanOrder = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.ElongOrderID == orderId).Select(u => new { OrderNum = u.dingdan_num, SpecialMemo = u.SpecialMemo, Num = u.num }).First();
                string original = string.Empty;

                if (dingdanOrder != null)
                {
                    source = "dingdan_info";
                    pay_amount = Convert.ToDecimal(dingdanOrder.SpecialMemo) * dingdanOrder.Num;
                    orderNum = dingdanOrder.OrderNum;
                }
                else
                {
                    string sql = string.Format("select * from bigtree..TB_hotelcashorder where sourceOrderID='{0}'", orderId);
                    var hotelcashorder = SqlSugarContext.TravelskyInstance.SqlQueryable<TB_hotelcashorder>(sql).Where(u => u.sourceOrderID == orderId).First();
                    if (hotelcashorder != null)
                    {
                        source = "TB_hotelcashorder";
                        pay_amount = hotelcashorder.totalPrice;
                        orderNum = hotelcashorder.orderNO;
                    }
                }

                if (pay_amount == 0)
                {
                    is_success = false.ToString().ToLower();
                    err_code = "-1";
                    err_msg = "获取价格为0";
                }
                else
                {
                    string beizhu = string.Format("<br/>【携程】:[支付通知]-支付给携程￥{0}  [{1}]", pay_amount, DateTime.Now.ToString());
                    noticeLogWriter.WriteOrder(orderId, "支付回调：支付携程金额{0}！", pay_amount);
                    string sql = string.Empty;
                    var isSuccess = false;
                    if (source == "dingdan_info")
                    {
                        sql = string.Format("update dingdan_info set beizhu=isnull(beizhu,'')+'{0}'  where dingdan_num='{1}'", beizhu, orderNum);
                        isSuccess = SqlSugarContext.TravelskyInstance.Ado.ExecuteCommand(sql) > 0;
                    }
                    else if (source == "TB_hotelcashorder")
                    {
                        sql = string.Format("update bigtree..tb_hotelcashorder set remark=isnull(remark,'')+'{0}' where orderno='{1}' ", beizhu, orderNum);
                        isSuccess = SqlSugarContext.TravelskyInstance.Ado.ExecuteCommand(sql) > 0;
                    }
                    if (!isSuccess)
                    {
                        noticeLogWriter.WriteOrder(orderId, "支付回调，保存数据库失败！", pay_amount);
                    }
                }

                resultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { is_success = is_success, pay_amount = pay_amount, pay_currency = pay_currency, remark = remark, err_code = err_code, err_msg = err_msg });
            }
            catch (Exception ex)
            {
                noticeLogWriter.WriteOrder(orderId, "支付回调，出现异常：{0}！", ex.ToString());
                resultJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { is_success = false.ToString().ToLower(), pay_amount = 0, pay_currency = pay_currency, remark = remark, err_code = "-1", err_msg = "编译出错" });
            }

            noticeLogWriter.WriteOrder(orderId, "支付回调，分销商返回：{0}！", resultJson);
            return resultJson;
        }
        #endregion

        private class KResult
        {
            public string code { get; set; }
            public string message { get; set; }
            public bool suc { get; set; }
        }
    }
}
