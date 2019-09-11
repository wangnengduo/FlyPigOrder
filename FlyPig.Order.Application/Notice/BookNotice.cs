
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FlyPig.Order.Core;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Core.Model;
using FlyPig.Order.Application.Entities.Dto;
using System.Text.RegularExpressions;
using Flypig.Order.Application.Order.Entities;
using FlyPig.Order.Framework.Logging;
using Newtonsoft.Json;
using FlyPig.Order.Framework.HttpWeb;

namespace FlyPig.Order.Application.Order.Notice
{
    /// <summary>
    /// 下单通知
    /// </summary>
    public class BookNotice : BaseOrderNotice<BookRQ, BookRQResult>
    {
        private readonly LogWriter logWriter;

        protected override string ChannelDiscernmentCode
        {
            get
            {
                return Request.RatePlanCode;
            }
        }

        public BookNotice(ShopType shop) : base(shop)
        {
            logWriter = new LogWriter("Tmall/Book");
        }

        /// <summary>
        /// 通知回调
        /// </summary>
        /// <returns></returns>
        protected override BookRQResult Notice()
        {
            BookRQResult result = new BookRQResult();
            result.ResultCode = "-100";
            result.Message = "生成订单失败";

            if (Shop == ShopType.ShengLv && Channel == ProductChannel.BigTree)
            {
                return result;
            }

            try
            {
                #region 异步保存发票信息
                if (Request.InvoiceInfo != null)
                {
                    try
                    {
                        Task.Run(() =>
                        {
                            SaveInvoiceInfo(Request);    //异步保存发票信息
                        });
                    }
                    catch (Exception ex)
                    {
                        logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "保存发票信息失败:{0},错误：{1}", JsonConvert.SerializeObject(Request.InvoiceInfo), ex.ToString());
                    }
                }

                #endregion

                FilterRemark(Request);
                TmallOrderDto order = ParseOrder(Request);
                if (Convert.ToInt32(order.hotelID) > 0)
                {
                    string hotelId = order.hotelID;
                    //Task.Factory.StartNew(() =>
                    //{
                    //    if (Shop == ShopType.RenXing || Shop == ShopType.YinJi)
                    //    {
                    //        string url = string.Format("http://47.106.239.82/MtPricePro/AliChek.ashx?key=qoeutpzmbjhsyehgixjsngklzoeuyhbgfs&hid=0&code={0}&checkin={1}&checkout={2}&shop=yj&visit=验单失败或下单成功&invoice=1&addplan=0", hotelId, Request.CheckIn, Request.CheckOut);
                    //        WebHttpRequest.Get(url);
                    //    }
                    //    //TODO 下单失败，关房
                    //});

                    if (orderRepository.IsExists(Request.TaoBaoOrderId))
                    {
                        result.Message = "重复预定";
                        result.ResultCode = "-106";
                        return result;
                    }

                    Task<TmallHotelPriceInfo> getHotelPriceInfo = Task.Factory.StartNew(
                        () => productChannel.GetHotelPriceInfo(new BookingCheckInputDto
                        {
                            CheckIn = Request.CheckIn,
                            CheckOut = Request.CheckOut,
                            HotelId = order.hotelID,
                            RatePlanId = order.ratePlanID,
                            RoomNum = order.roomNum,
                            RoomTypeId = order.roomID,
                            RatePlanCode= Request.RatePlanCode
                        }
                        ));
                    getHotelPriceInfo.Wait();
                    var simpleOrder = getHotelPriceInfo.Result;
                    if (simpleOrder == null || string.IsNullOrEmpty(simpleOrder.PriceStr))
                    {
                        //Task.Factory.StartNew(() =>
                        //{
                        //    var client = DomesticHotelClient.CreateInstance(Shop, Channel);
                        //    client.RoomRate.UpdateRoomRateByHid(hotelId);
                        //});

                        //Task.Factory.StartNew(() =>
                        //{
                        //    //异步关闭所有房型
                        //    RoomRateService roomRateService = new RoomRateService(Shop, Channel);
                        //    roomRateService.UpdateRoomRateByHid(order.hotelID, true, true);
                        //});

                        //预订时，进订为不可预订时关房
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                AliTripValidate av = new AliTripValidate();
                                av.CheckInTime = Request.CheckIn;
                                av.CheckOutTime = Request.CheckOut;
                                av.RatePlanCode = Request.RatePlanCode;
                                av.HotelId = hotelId;
                                av.RoomId = order.roomID;
                                av.RatePlanId = order.ratePlanID;
                                av.CreateTime = DateTime.Now;
                                av.IsFull = true;
                                av.Channel = (int)Channel;
                                av.Shop = (int)Shop;
                                av.Remark = "预订满房";
                                SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();

                                string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source={1}", hotelId, (int)Channel);
                                WebHttpRequest.Get(url);
                            }
                            catch
                            {
                            }
                        });

                        //当携程预订满房时保存数据库返回下单成功为客服提醒客户申请退款
                        if ((int)Channel == 8)
                        {
                            
                            string priceStr = string.Empty;
                            
                            string RoomNameCn = simpleOrder.RoomName;
                            
                            
                            order.hotelName = simpleOrder.HotelName;
                            order.roomName = simpleOrder.RoomName;
                            //order.ratePlanName = simpleOrder.RatePlanName;
                            //order.totalPrice = Convert.ToDecimal(Request.TotalPrice- Request.VoucherInfos[0].VoucherPomotionAmt) / 100;
                            order.paymentType = (short)Request.PaymentType;
                            order.totalPrice = 0;
                            order.RatePlanCode = Request.RatePlanCode;
                            order.taobaoTotalPrice = Convert.ToDecimal(Request.TotalPrice) / 100;
                            order.CurrencyCode = "CNY";



                            //if (Request.RatePlanCode.Split('_').Length > 3)
                            //{
                            //    order.ratePlanName = string.Format("{0}[虚拟]", order.ratePlanName);
                            //}


                            if (Request.ContactName.Length > 20)
                            {
                                order.remark += string.Format("【系统】[注意：联系人-{0}]     <br/>{1}", order.contactName, order.remark);
                                order.contactName = order.contactName.Substring(0, 20);
                            }

                            if (order.guestName.Length > 64)
                            {
                                order.remark += string.Format("【系统】[注意：入住人-{0}]     <br/>{1}", order.guestName, order.remark);
                                order.guestName = order.guestName.Substring(0, 64);
                            }

                            try
                            {
                                var serviceResult = orderRepository.SaveOrder(Channel, order);
                                if (serviceResult.IsSucess)
                                {
                                    result.Message = string.Format("创建订单成功");
                                    result.ResultCode = "0";
                                    result.OrderId = order.orderNO;
                                }
                                else
                                {
                                    result.Message = string.Format("系统保存订单失败:{0}", serviceResult.Message);
                                    result.ResultCode = "-105";
                                    logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "保存订单表失败");
                                }
                            }
                            catch (Exception ex)
                            {
                                result.Message = string.Format("系统保存异常");
                                result.ResultCode = "-104";
                                logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "保存订单表失败：" + ex.ToString());
                            }
                        }
                        else
                        {
                            result.Message = string.Format("满房，不可预定");
                            result.ResultCode = "-101";
                        }
                        logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "::创建订单失败(满房)-编号:{0},{1},失败原因{2}", hotelId, Request.RatePlanCode, result.Message);
                        return result;
                    }

                    if (string.IsNullOrEmpty(simpleOrder.RoomName))
                    {
                        //Task.Factory.StartNew(() =>
                        //{
                        //    //异步关闭所有房型
                        //    RoomRateService roomRateService = new RoomRateService(Shop, Channel);
                        //    roomRateService.UpdateRoomRateByHid(order.hotelID, true, true);
                        //});
                        logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "获取不到房型；");
                        result.Message = string.Format("房型不存在");
                        result.ResultCode = "-114";
                        return result;
                    }

                    if (!CheckPrice(Request.TotalPrice, simpleOrder.DatePrice, Request.RoomNum))   //校验价格
                    {
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                AliTripValidate av = new AliTripValidate();
                                av.CheckInTime = Request.CheckIn;
                                av.CheckOutTime = Request.CheckOut;
                                av.RatePlanCode = Request.RatePlanCode;
                                av.HotelId = hotelId;
                                av.RoomId = order.roomID;
                                av.RatePlanId = order.ratePlanID;
                                av.CreateTime = DateTime.Now;
                                av.IsFull = true;
                                av.Channel = (int)Channel;
                                av.Shop = (int)Shop;
                                av.Remark = "下单总价校验失败";
                                SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();

                                //47.106.132.129:9078
                                string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source={1}", hotelId, (int)Channel);
                                WebHttpRequest.Get(url);
                            }
                            catch
                            {
                            }
                            ////异步关闭所有房型
                            //RoomRateService roomRateService = new RoomRateService(Shop, Channel);
                            //roomRateService.UpdateRoomRateByHid(order.hotelID, true, true);
                        });

                        logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "下单价格异常{0}-{1}；完整:{2}", (Request.TotalPrice / 100), simpleOrder.DatePrice * Request.RoomNum, JsonConvert.SerializeObject(simpleOrder));
                        result.Message = string.Format("下单总价校验失败");
                        result.ResultCode = "-103";
                        return result;
                    }
                    string RoomName = simpleOrder.RoomName;
                    if (simpleOrder.RoomName == "获取房型失败")
                    {
                        RoomName = simpleOrder.RatePlanName.Replace("[携程开具发票]", "").Replace("[酒店开具发票]", "").Replace("[金牌]","");
                    }

                    order.sTotalPrice = simpleOrder.STotalPrice * Request.RoomNum;
                    order.priceStr = simpleOrder.PriceStr;
                    order.hotelName = simpleOrder.HotelName;
                    order.roomName = RoomName;
                    order.ratePlanName = simpleOrder.RatePlanName;
                    order.totalPrice = Convert.ToDecimal(simpleOrder.DatePrice * Request.RoomNum);
                    order.paymentType = (short)simpleOrder.PaymentType;
                    order.taobaoTotalPrice = Convert.ToDecimal(Request.TotalPrice) / 100;
                    order.CurrencyCode = simpleOrder.CurrencyCode;
                    order.RatePlanCode = Request.RatePlanCode;

                    if (simpleOrder.CurrencyPrice.HasValue)
                    {
                        order.MemberPrice = simpleOrder.CurrencyPrice;
                    }

                    if (string.IsNullOrEmpty(simpleOrder.RatePlanName))
                    {
                        order.remark += " [rp不存在]";
                    }


                    //if (Request.RatePlanCode.Split('_').Length > 3)
                    //{
                    //    order.ratePlanName = string.Format("{0}[虚拟]", order.ratePlanName);
                    //}


                    if (Request.ContactName.Length > 20)
                    {
                        order.remark += string.Format("【系统】[注意：联系人-{0}]     <br/>{1}", order.contactName, order.remark);
                        order.contactName = order.contactName.Substring(0, 20);
                    }

                    if (order.guestName.Length > 64)
                    {
                        order.remark += string.Format("【系统】[注意：入住人-{0}]     <br/>{1}", order.guestName, order.remark);
                        order.guestName = order.guestName.Substring(0, 64);
                    }

                    try
                    {
                        var serviceResult = orderRepository.SaveOrder(Channel, order);
                        if (serviceResult.IsSucess)
                        {
                            //异步更新买家信息
                            Task.Factory.StartNew(() =>
                            {
                                UpdateBuyerInfo(Shop, order.taoBaoOrderId);
                            });

                            result.Message = string.Format("创建订单成功");
                            result.ResultCode = "0";
                            result.OrderId = order.orderNO;
                        }
                        else
                        {
                            result.Message = string.Format("系统保存订单失败:{0}", serviceResult.Message);
                            result.ResultCode = "-105";
                            logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "保存订单表失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Message = string.Format("系统保存异常");
                        result.ResultCode = "-104";
                        logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "保存订单表失败：" + ex.ToString());
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                logWriter.WriteOrder(Request.TaoBaoOrderId.ToString(), "创建订单异常：{0}", ex.ToString());
                result.ResultCode = "-104";
                result.Message = "系统异常";
                return result;
            }
        }

        #region 过滤特殊备注
        /// <summary>
        /// 过滤特殊备注
        /// </summary>
        /// <param name="order"></param>
        public static void FilterRemark(BookRQ order)
        {
            if (order != null && !string.IsNullOrEmpty(order.Comment))
            {
                string result = Regex.Replace(order.Comment, "【飞猪订单】预付订单，参加.*活动，", "");
                result = Regex.Replace(result, "使用了[\\d]{1,3}.[\\d]{2}元优惠。如结账金额有变化，", "");
                result = Regex.Replace(result, "请按结账金额减[\\d]{1,3}.[\\d]{2}开票。", "");
                result = Regex.Replace(result, "卖家优惠活动：[\\d]{1,3}.[\\d]{2}元", "");
                order.Comment = result;
            }
        }
        #endregion

        #region 更新买家信息
        /// <summary>
        /// 更新买家信息
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="taobaoOrderId"></param>
        public void UpdateBuyerInfo(ShopType shop, long taobaoOrderId)
        {
            if (shop == ShopType.ShengLv)
            {
                return;
            }

            var orderSearchResult = aliTripOrderClient.GetTaoBaoOrderInfo(taobaoOrderId);
            if (orderSearchResult != null && orderSearchResult.HotelOrders.Count > 0)
            {
                var order = orderSearchResult.HotelOrders.FirstOrDefault();
                string buyNick = order.BuyerNick;


                string sql = string.Format("update TB_hotelcashorder set BuyerNick='{0}' where taoBaoOrderId={1}", buyNick, taobaoOrderId);
                SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql);
            }
        }
        #endregion

        #region 校验金额
        /// <summary>
        /// 校验金额
        /// </summary>
        /// <param name="baoFang"></param>
        /// <param name="isPromote"></param>
        /// <param name="totalPrice"></param>
        /// <param name="datePrice"></param>
        /// <param name="roomNum"></param>
        /// <returns></returns>
        private bool CheckPrice(long totalPrice, decimal datePrice, int roomNum)
        {
            long payAmount = totalPrice;

            if (Request.VoucherInfos != null && Request.VoucherInfos.Count > 0)
            {
                payAmount = payAmount - Request.VoucherInfos[0].VoucherPomotionAmt;
            }

            if (Channel == ProductChannel.BigTree)
            {
                return true;     //自签放开限制
            }

            if (Channel == ProductChannel.Ctrip)
            {
                if ((payAmount / 100) < (datePrice * roomNum) * 0.95m)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            if ((payAmount / 100) < (datePrice * roomNum) * 0.98m)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 生成订单号
        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <param name="ratePlanCode"></param>
        /// <returns></returns>
        private string GenerateOrderNum(string ratePlanCode)
        {
            string orderPrefix = string.Empty;

            if (Channel == ProductChannel.Ctrip)
            {
                if (Shop == ShopType.LingZhong)
                {
                    orderPrefix = "c";
                }
                else if (Shop == ShopType.RenXing)
                {
                    orderPrefix = "rc";
                }
                else if (Shop == ShopType.YinJi)
                {
                    orderPrefix = "yc";
                }
                else if (Shop == ShopType.ShengLv)
                {
                    orderPrefix = "cs";
                }
                else if (Shop == ShopType.RenNiXing)
                {
                    orderPrefix = "xr";
                }
            }
            else if (Channel == ProductChannel.MT)
            {
                if (Shop == ShopType.LingZhong)
                {
                    orderPrefix = "m";
                }
                else if (Shop == ShopType.YinJi)
                {
                    orderPrefix = "ym";
                }
                else if (Shop == ShopType.ShengLv)
                {
                    orderPrefix = "ms";
                }
                else if (Shop == ShopType.RenXing)
                {
                    orderPrefix = "rm";
                }
                else if (Shop == ShopType.RenNiXing)
                {
                    orderPrefix = "mr";
                }
            }
            else if (Channel == ProductChannel.BigTree)
            {
                if (Shop == ShopType.LingZhong)
                {
                    orderPrefix = "w";
                }
                else if (Shop == ShopType.YinJi)
                {
                    orderPrefix = "Y";
                }
                else if (Shop == ShopType.RenXing)
                {
                    orderPrefix = "RF";
                }
                else if (Shop == ShopType.YinJiGuoLv)
                {
                    orderPrefix = "yg";
                }
            }
            else if (Channel == ProductChannel.MTNew)
            {
                orderPrefix = "mn";
            }
            else if (Channel == ProductChannel.Elong)
            {
                if (Shop == ShopType.RenXing)
                {
                    orderPrefix = "er";
                }
            }
            else
            {
                throw new ArgumentException("OrderChannel is not choose..", "OrderChannel");
                #region 隐藏代码
                //if (ratePlanCode.Contains("wl"))
                //{
                //    return "w" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("h"))
                //{
                //    return "q" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("rt"))
                //{
                //    return "t" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("yj"))
                //{
                //    return "Y" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("el"))
                //{
                //    return "E" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("mt"))
                //{
                //    return "m" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("tx"))
                //{
                //    return "ym" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("ey"))
                //{
                //    return "EL" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("jl"))
                //{
                //    return "J" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("ms"))
                //{
                //    return "ms" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("rz"))
                //{
                //    return "RF" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //}
                //else if (ratePlanCode.Contains("xl"))
                //{
                //    return "x" + DateTime.Now.ToString("yyMMddHHmmssfff");
                //} 
                #endregion
            }

            return string.Format("{0}{1}", orderPrefix, DateTime.Now.ToString("yyMMddHHmmssfff"));
        }
        #endregion

        #region 转换订单
        /// <summary>
        /// 转换订单
        /// </summary>
        /// <param name="bookRQ"></param>
        /// <returns></returns>
        private TmallOrderDto ParseOrder(BookRQ bookRQ)
        {
            TmallOrderDto order = new TmallOrderDto();
            order.checkInDate = bookRQ.CheckIn.Date;
            order.checkOutDate = bookRQ.CheckOut.Date;
            order.contactName = bookRQ.ContactName;
            order.contactTel = bookRQ.ContactTel;
            order.orderNO = GenerateOrderNum(bookRQ.RatePlanCode);
            order.TaoBaoHotelId = bookRQ.TaoBaoHotelId;
            order.TaoBaoRoomTypeId = bookRQ.TaoBaoRoomTypeId;
            order.TaoBaoRatePlanId = bookRQ.TaoBaoRatePlanId;
            var hotelInfo = GetIdInfoByRpCode(bookRQ.RatePlanCode);
            if (hotelInfo != null)
            {
                order.hotelID = hotelInfo.HotelId;
                order.roomID = hotelInfo.RoomTypeId;
                order.ratePlanID = hotelInfo.RatePlanId;
            }
            else
            {
                throw new Exception("价格计划ID有误");
            }

            if (bookRQ.OrderGuests != null)
            {
                List<string> stg = new List<string>();
                for (int i = 0, l = bookRQ.OrderGuests.Count; i < l; i++)
                {
                    stg.Add(bookRQ.OrderGuests[i].Name);
                }
                order.guestName = string.Join(",", stg.ToArray());
            }
            else
            {
                //      order.guestName = order.guestName;
            }
            try
            {
                //  特殊处理单号
                string ns = order.contactTel.Replace("*", "0").Replace("-", "0").Replace("_", "0");
                if (ns.Length > 3)
                {
                    ns = ns.Substring(ns.Length - 3);
                    order.orderNO = order.orderNO + ns;
                }
            }
            catch
            {
            }

            order.caozuo = string.Format("system 添加订单 [{0}]  \r\n", DateTime.Now.ToString());
            order.earliestArriveTime = bookRQ.EarliestArriveTime;
            order.lastArriveTime = bookRQ.LatestArriveTime;
            order.guestMobile = bookRQ.ContactTel;
            order.roomNum = bookRQ.RoomNum;
            order.taoBaoGId = bookRQ.TaoBaoGid;
            order.taoBaoOrderId = bookRQ.TaoBaoOrderId;
            order.totalPrice = Convert.ToInt32(bookRQ.TotalPrice / 100);
            order.comment = bookRQ.Comment;
            order.payTime = new DateTime(1900, 1, 1);
            order.orderTime = DateTime.Now;
            order.sCheckInDate = new DateTime(1990, 1, 1);
            order.sCheckOutDate = new DateTime(1990, 1, 1);
            order.tradeStatus = "";
            order.logisticsStatus = "";
            order.alipayPay = 0;
            order.state = Channel == ProductChannel.BigTree ? 66 : 1;
            order.payType = "t";
            order.shopType = (short)Shop;
            order.source = "";
            order.Refuse = 0;
            order.sentfaxtime = new DateTime(1900, 1, 1);
            order.orderType = Convert.ToInt16(Channel.GetDescription());
            order.DailyInfoPrice = JsonConvert.SerializeObject(bookRQ.DailyInfos);
            return order;
        }
        #endregion

        #region 保存发票信息
        /// <summary>
        /// 保存发票信息
        /// </summary>
        /// <param name="bq"></param>
        private void SaveInvoiceInfo(BookRQ bq)
        {
            try
            {
                AliTripInvoice invoice = new AliTripInvoice();
                invoice.Bank = bq.InvoiceInfo.Bank;
                invoice.TaoBaoOrderId = bq.TaoBaoOrderId;
                invoice.BankAccount = bq.InvoiceInfo.BankAccount;
                invoice.InvoiceTitle = bq.InvoiceInfo.InvoiceTitle;
                invoice.Comment = bq.InvoiceInfo.Comment;
                invoice.WantTime = bq.InvoiceInfo.WantTime;
                invoice.CompanyTax = bq.InvoiceInfo.CompanyTax;
                invoice.CompanyTel = bq.InvoiceInfo.CompanyTel;
                invoice.EarllyPrepare = bq.InvoiceInfo.EarllyPrepare;
                invoice.InvoiceType = bq.InvoiceInfo.InvoiceType;
                invoice.NeedInvoice = bq.InvoiceInfo.NeedInvoice;
                invoice.PostType = Convert.ToInt32(bq.InvoiceInfo.PostType);
                invoice.ReceiverAddress = bq.InvoiceInfo.ReceiverAddress;
                invoice.ReceiverMobile = bq.InvoiceInfo.ReceiverMobile;
                invoice.ReceiverName = bq.InvoiceInfo.ReceiverName;
                invoice.RegisterAddress = bq.InvoiceInfo.RegisterAddress;
                invoice.SubmitTime = bq.InvoiceInfo.SubmitTime;


                var flag = SqlSugarContext.ResellbaseInstance.Insertable(invoice).ExecuteCommand() > 0;
                if (!flag)
                {
                    logWriter.WriteOrder(bq.TaoBaoOrderId.ToString(), "保存发票到数据库失败:{0}", bq.TaoBaoOrderId);
                }
            }
            catch (Exception ex)
            {
                logWriter.WriteOrder(bq.TaoBaoOrderId.ToString(), "{0} 保存发票信息出现异常，具体情况:{1}", bq.TaoBaoOrderId, ex.ToString());
            }
        }


        #endregion

    }


}
