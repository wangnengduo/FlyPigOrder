using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order.Message;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Repository.Order;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Order.Channel
{
    public class ChenYiMeiTuanOrderChannel : TmallMeiTuanOrderChannel, IMeiTuanOrderChannel
    {

        public ChenYiMeiTuanOrderChannel() : this(ShopType.RenNiXing)
        {
        }

        public ChenYiMeiTuanOrderChannel(ShopType shop) : base(shop)
        {
        }

        public override string RequestUrl => "http://211.147.242.35:8085";


        /// <summary>
        /// 同步订单到辰亿
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public override ServiceResult SynChannelOrder(int aid)
        {
            var result = new ServiceResult();
            string orderId = DateTime.Now.ToString("yyMMddHHmmssfff") + random.Next(1000, 9999) + random.Next(10, 99);
            try
            {
                var order = SqlSugarContext.ResellbaseInstance.Queryable<TB_hotelcashorder>().Where(u => u.aId == aid).First();

                if (order.alipayPay == 0)
                {
                    return result.SetError("订单未支付，无法下单到渠道！");
                }

                if (order.totalPrice==0)
                {
                    string remark = string.Format("【系统】:下单到携程失败（已售完或扣减库存失败） [{0}]<br/>", DateTime.Now.ToString());
                    orderRepository.UpdateRemarkState(aid, 63, remark);

                    int NowHour = DateTime.Now.Hour;//当前时间的时数
                    int NowMinute = DateTime.Now.Minute;//当前时间的分钟数
                    //晚上12点半后，8点前满房时自动发送短信
                    if ((NowHour == 0  && NowMinute > 30) || (NowHour > 0 && NowHour < 8))
                    {
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                //发满房短信（发送满房信息）
                                FlyPigMessageUility.ThirdCommunication(Shop, Convert.ToInt32(order.aId), "系统", 1);
                            }
                            catch
                            {

                            }
                        });
                    }
                    //string caozuo = string.Format("<br/>【系统】已发送满房短信[{0}]", DateTime.Now.ToString());
                    ////发满房短信
                    //FlyPigMessageUility.ThirdCommunication(Shop, Convert.ToInt32(order.aId), "系统", 1);
                    ////更新状态、备注及操作
                    //orderRepository.UpdateRemarkStateCaoZuo(order.aId, 63, remark, caozuo);

                    return result.SetError("订单底价为0，无法下单到渠道！");
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
                    return result.SetSucess("该订单已提交到CY，请勿重复提交");
                }


                var din = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.fax == order.taoBaoOrderId.ToString()).First();
                if (din == null)
                {
                    din = new dingdan_info();
                }
                else
                {
                    order.sourceOrderID = din.dingdan_num;
                    var excuteRow = SqlSugarContext.ResellbaseInstance.Updateable(order).ExecuteCommand();
                    if (excuteRow == 0)
                    {
                        //TODO  更新订单失败
                    }
                    return result.SetError("已存在单号({0}),请不要重复下单", din.dingdan_num);
                }
                din.typeid = "0900";
                din.username = order.contactName;
                din.userid = 0;
                din.dingdan_num = orderId;
                din.HotelID = order.hotelID;
                din.RoomTypeID = order.roomID;
                din.RatePlanID = order.ratePlanID;
                din.hotel_name = order.hotelName;
                din.city_id = 0;
                din.room_id = Convert.ToInt32(order.roomID);
                din.room_name = string.Format("({0}){1}", order.roomName, order.ratePlanName);
                din.num = order.roomNum;
                din.begin_date = order.checkInDate;
                din.end_date = order.checkOutDate;
                din.dingdan_date = DateTime.Now;
                din.tel = "170000000000"; //order.contactTel;      
                din.personname = order.contactName;
                din.fax = order.taoBaoOrderId.ToString();
                din.Version = 2;

                //  din.total = Convert.ToInt32(order.totalPrice);
                // din.personnum = order.contactName.Split(',').Length;
                din.cause = "RMB";
                din.caozuo = string.Format("系统于{0}添加订单\n", DateTime.Now.ToString());
                din.snote = order.comment;
                din.beizhu = "";
                din.price_str = order.priceStr;
                // din.pingtai = "xl";

                string prefix = string.Empty;
                if (order.shopType == 5)
                {
                    prefix = "xl";
                }
                else if (order.shopType == 6)
                {
                    prefix = "yj";
                }
                else if (order.shopType == 8)
                {
                    prefix = "rx";
                }
                else if (order.shopType == 9)
                {
                    prefix = "rnx";
                }

                din.pingtai = prefix;

                var payPrice = order.alipayPay;
                if (order.orderType == 5)
                {
                    payPrice = order.totalPrice;
                }

                din.ly_pingtai = string.Format("{3}:{0}:{1}:ip:127.0.0.1:cash::money:{2}", order.taoBaoOrderId, din.dingdan_date.ToString("yyyyMMddHHmmss"), order.alipayPay, prefix);
                din.SpecialMemo = Convert.ToDecimal(order.totalPrice / order.roomNum).ToString();
                din.roomtype = Convert.ToDecimal((order.totalPrice + order.sTotalPrice) / order.roomNum).ToString();

                if (order.orderType == 12)
                {
                    din.PriceOrderWay = 42;
                }
                else if (order.orderType == 5)
                {
                    din.PriceOrderWay = 66;
                    // din.PriceOrderWay = 16;
                }
                else if (order.orderType == 15)
                {
                    din.PriceOrderWay = 16;
                }
                else if (order.orderType == 16)
                {
                    din.PriceOrderWay = 86;
                }
                else
                {
                    din.PriceOrderWay = 52;
                }

                din.state = 1;
                din.ExtraMoney = 0;
                din.PayMessage = Convert.ToDecimal(order.sTotalPrice / order.roomNum).ToString();
                int rows = SqlSugarContext.TravelskyInstance.Insertable(din).ExecuteCommand();
                if (rows == 0)
                {
                    //TODO  更新订单失败
                }

                order.sourceOrderID = orderId;
                //<br/>【系统】:同步订单成功,订单号({1}) [{2}]
                order.remark = string.Format("{0}【系统】:同步订单成功,订单号({1}) [{2}]<br/>", order.remark, orderId, DateTime.Now.ToString());
                rows = SqlSugarContext.ResellbaseInstance.Updateable(order).ExecuteCommand();
                if (rows == 0)
                {
                    string sql = string.Format("delete dingdan_info where dingdan_num='{0}' ", orderId);
                    SqlSugarContext.TravelskyInstance.Ado.ExecuteCommand(sql);
                    return result.SetError("同步订单失败");
                }
                else
                {

                    order.state = 3;
                    var excuteRow = SqlSugarContext.ResellbaseInstance.Updateable(order).ExecuteCommand();

                    return result.SetSucess("同步订单成功,订单号({0})", orderId);
                }

            }
            catch (Exception ex)
            {
                string sql = string.Format("delete dingdan_info where dingdan_num='{0}' ", orderId);
                SqlSugarContext.TravelskyInstance.Ado.ExecuteCommand(sql);
                return result.SetError("同步订单到数据库失败，出现异常：{0}", ex.Message);
            }
        }



        public override bool UpdateCancelRemark(long taobaoOrderId)
        {
            return new ShengLvOrderRepository().UpdateCancelRemark(taobaoOrderId);
        }
 
    }
}
