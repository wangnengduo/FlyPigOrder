using Flypig.Order.Application.Common;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Repository.Order;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Order
{
    public class ShengLvMeiTuanOrderChannel : TmallMeiTuanOrderChannel
    {
        public ShengLvMeiTuanOrderChannel() : this(ShopType.ShengLv)
        {
        }

        public ShengLvMeiTuanOrderChannel(ShopType shop) : base(shop)
        {
        }

        public override string RequestUrl => "http://47.96.68.32";


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

                if (order == null)
                {
                }

                if (order.alipayPay == 0)
                {
                    return result.SetError("订单未支付，无法下单到渠道！");
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
                    return result.SetError("该订单已提交到CY，请勿重复提交");
                }


                var din = SqlSugarContext.ResellbaseInstance.Queryable<dingdan_info>().Where(u => u.fax == order.taoBaoOrderId.ToString()).First();
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
                        //TODO  更新订单状态失败
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
                din.room_name = order.roomName;
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
                din.pingtai = "lz";

                var payPrice = order.alipayPay;
                if (order.orderType == 5)
                {
                    payPrice = order.totalPrice;
                }

                din.ly_pingtai = string.Format("lz:{0}:{1}:ip:127.0.0.1:cash::money:{2}", order.taoBaoOrderId, din.dingdan_date.ToString("yyyyMMddHHmmss"), order.alipayPay);
                din.SpecialMemo = Convert.ToDecimal(order.totalPrice / order.roomNum).ToString();
                din.roomtype = Convert.ToDecimal((order.totalPrice + order.sTotalPrice) / order.roomNum).ToString();

                din.PriceOrderWay = 42;
                //if (order.orderType == 11)
                //{
                //    din.PriceOrderWay = 42;
                //}
                //else if (order.orderType == 12)
                //{
                //    din.PriceOrderWay = 42;
                //}
                //else if (order.orderType == 5)
                //{
                //    din.PriceOrderWay = 66;
                //}
                //else
                //{
                //    din.PriceOrderWay = 52;
                //}

                din.state = 1;
                din.ExtraMoney = 0;
                din.PayMessage = Convert.ToDecimal(order.sTotalPrice / order.roomNum).ToString();
                int rows = SqlSugarContext.ResellbaseInstance.Insertable(din).ExecuteCommand();
                if (rows == 0)
                {
                    // RabbitClient.Instance.Send(RabbitCommonKey.ChenYiOrderFaild, din);
                }

                order.sourceOrderID = orderId;
                order.remark = string.Format("{0}<br/>【系统】:同步订单成功,订单号({1}) [{2}]", order.remark, orderId, DateTime.Now.ToString());
                rows = SqlSugarContext.ResellbaseInstance.Updateable(order).ExecuteCommand();
                if (rows == 0)
                {
                    string sql = string.Format("delete sub_info where dingdan_num='{0}' ", orderId);
                    SqlSugarContext.ResellbaseInstance.Ado.ExecuteCommand(sql);
                    return result.SetError("同步订单失败");
                }
                else
                {
                    //string sql = string.Format("delete sub_info where dingdan_num='{0}' ", orderId);
                    //SqlSugarContext.TravelskyInstance.Ado.ExecuteCommand(sql);
                    //  order.orderType = 13;

                    order.state = 24;
                    var excuteRow = SqlSugarContext.ResellbaseInstance.Updateable(order).ExecuteCommand();
                    if (excuteRow == 0)
                    {
                        //   RabbitClient.Instance.Send(RabbitCommonKey.ElongFaildOrder, order);
                    }

                    //更新orderType
                    return result.SetSucess("同步订单成功,订单号({0})", orderId);
                }

            }
            catch (Exception ex)
            {
                string sql = string.Format("delete sub_info where dingdan_num='{0}' ", orderId);
                SqlSugarContext.ResellbaseInstance.Ado.ExecuteCommand(sql);

             //   new LogWriter("MeiTuan/Test").Write(ex.ToString());

                return result.SetError("同步订单到数据库失败，出现异常：{0}", ex.Message);
            }
        }

        public override bool UpdateCancelRemark(long taobaoOrderId)
        {
            return new ChenYiOrderRepository().UpdateCancelRemark(taobaoOrderId);
        }
    }
}
