
using Flypig.Order.Application.Common;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Framework.Common;
using FlyPig.Order.Application.Hotel.Channel;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flypig.Order.Application.Order.Message
{
    public class FlyPigMessageUility
    {
        #region 第三方确认
        /// <summary>
        /// 第三方确认
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        public static ServiceResult ThirdCenterMessage(ShopType shop, long aid)
        {
            var result = new ServiceResult();

            try
            {
                var sqlSugar = SqlSugarContext.BigTreeInstance;
                if (shop == ShopType.ShengLv || shop == ShopType.RenNiXing)
                {
                    sqlSugar = SqlSugarContext.ResellbaseInstance;
                }

                var order = sqlSugar.Queryable<TB_hotelcashorder>().Where(u => u.aId == aid).First();
                string orderNo = string.Empty;
                string shopInfo = order.shopType == 5 ? "遨游盛旅" : "贵州任你行";
                string phone = order.shopType == 5 ? "0851-88574658" : "0851-88574658";

                if (order.orderType == 11 && order.shopType != 7)
                {
                    var channelOrder = SqlSugarContext.ResellbaseInstance.Queryable<dingdan_info>().Where(u => u.fax == order.taoBaoOrderId.ToString()).First();
                    orderNo = channelOrder.ElongOrderID;
                }
                else if (order.orderType == 12 || order.orderType == 13 || order.orderType == 5)
                {
                    var channelOrder = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.fax == order.taoBaoOrderId.ToString()).First();
                    orderNo = channelOrder.ElongOrderID;
                }
                else
                {
                    orderNo = order.sourceOrderID;
                }


                if (order.checkInDate < DateTime.Now.Date.AddDays(-1))
                {
                    return result.SetError("已过期，无法发送");
                }


                string mobilePhone = "17374844623";
                if (order.orderType == 3)
                {
                    mobilePhone = "17374844623";
                }

                //string message = string.Format("订单号：（{0}），尊敬的{1}客人！您预订的{2}酒店已预订成功，酒店前台出示身份证+手机预订号{5} 交付押金办理入住，由于您预订的酒店是特价产品，无需提及预订渠道无需出示短信，祝您旅途愉快！办理入住如有问题，请联系旺旺或商家电话:{4} 【{3}】", orderNo, order.contactName, order.hotelName, shopInfo, phone, mobilePhone);
                //if (order.ratePlanName.Contains("酒店开具发票"))
                //{
                //    message = string.Format(" 尊敬的{1}客人！{2}已预订成功，请在前台出示身份证 + 订单号{0} + 手机预订号{5}办理入住，酒店前台开具发票，如无法开具发票、查不到订单或者其他办理入住问题，请联系我司协助处理，旺旺或商家电话:{4} 祝您旅途愉快！【{3}】", orderNo, order.contactName, order.hotelName, shopInfo, phone, mobilePhone);
                //}

                string message = string.Format("尊敬的{0}客人！您预订的{1}酒店已预订成功，订单号：（{4}）酒店前台出示身份证 交付押金办理入住，由于您预订的酒店是特价产品，无需提及预订渠道无需出示短信，祝您旅途愉快！办理入住如有问题，请联系旺旺或商家电话:{3} 【{2}】", order.contactName, order.hotelName, shopInfo, phone, orderNo);

                if (order.ratePlanName.Contains("酒店开具发票") /*&& order.orderType != 5*/)
                {
                    message = string.Format("尊敬的{0}客人！您预订{1}酒店已预订成功，订单号：（{4}）酒店前台出示身份证 交付押金办理入住，酒店前台可以开具发票，如无法开具发票可以联系我司协助处理，祝您旅途愉快！办理入住如有问题，请联系旺旺或商家电话:{3} 【{2}】", order.contactName, order.hotelName, shopInfo, phone, orderNo);
                }

                bool flag = false;
                string sendResult = string.Empty;
                if (shop == ShopType.ShengLv || shop == ShopType.RenNiXing)
                {
                    message = string.Format("您预订{0}酒店已确认，酒店查询编号为：{1} 请在前台出示身份证报订单入住人姓名即可入住。如需帮助请及时商家旺旺或热线电话18208503314【贵州任你行】", order.hotelName, order.sourceOrderID);
                    sendResult = SMSUilitily.SendShengLv(order.contactTel, message,1);
                    try
                    {
                        var match = Regex.Match(sendResult, "[0-9]{10,20}");
                        flag = Convert.ToInt64(match.Value) > 0;
                    }
                    catch
                    {
                        flag = false;
                    }
                    
                }
                else
                {
                    flag = SMSUilitily.Send(0, order.contactTel, message);
                }
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        string sql = string.Format("INSERT INTO [SMSRecord]([Phone],[Content],[Result],[CreateTime]) VALUES('{0}','{1}','{2}',getdate())", order.contactTel, message, sendResult);
                        SqlSugarContext.ResellbaseInstance.Ado.ExecuteCommand(sql);
                    }
                    catch
                    {
                    }
                });

                if (flag)
                {
                    string sql = string.Format("INSERT INTO [AliTripSMS]([taobaoOrderId],[Tell],[Content] ,[ShopType],[Source],[CreateTime])VALUES({2},'{0}','{1}',{3},5,getdate())", order.contactTel, message, order.taoBaoOrderId, order.shopType);
                    SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql);

                    var orderRepository = ProductChannelFactory.CreateOrderRepository(shop, ProductChannel.MT);
                    string caozuo = string.Format("{0} 发送{1}短信 ", "系统", "订单确认");
                    orderRepository.UpdateOrderCaoZuo(order.aId, caozuo);
                    return result.SetSucess("短信发送成功");
                }
                else
                {
                    return result.SetError("短信发送失败");
                }

               
            }
            catch (Exception ex)
            {
                return result.SetError("短信发送失败，请重试(如果遇到多次发送失败，请联系相关技术)");
            }
        }
        #endregion

        #region 第三方沟通短信
        /// <summary>
        /// 第三方沟通短信
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        public static ServiceResult ThirdCommunication(ShopType shop, long aid, string username, int type)
        {
            var result = new ServiceResult();
            try
            {
                var sqlSugar = SqlSugarContext.BigTreeInstance;
                if (shop == ShopType.ShengLv || shop == ShopType.RenNiXing)
                {
                    sqlSugar = SqlSugarContext.ResellbaseInstance;
                }
                var orderRepository = ProductChannelFactory.CreateOrderRepository(shop, ProductChannel.MT);
                var order = sqlSugar.Queryable<TB_hotelcashorder>().Where(u => u.aId == aid).First();

                string message = string.Empty;
                string shopPrefix = string.Empty;
                string phone = string.Empty;
                //if (order.shopType == 5)
                //{
                //    phone = "020-87375157";
                //    shopPrefix = "【凌众商旅】";
                //}
                //else if (order.shopType == 6)
                //{
                //    phone = "020-87370379";
                //    shopPrefix = "【印迹】";
                //}
                //else
                //{
                //    shopPrefix = "【遨游盛旅】";
                //}
                shopPrefix = "【贵州任你行】";


                if (type == 1)        //满房
                {
                    message = string.Format("{0}您好，您预订的{1}-{2} 经酒店回复无法确认订单。请及时选择退款原因：协商一致或行程变更退款，以免耽误您的行程。如有问题请致电{3}", order.contactName, order.hotelName, order.roomName, phone);
                }
                else if (type == 2)   //沟通
                {
                    message = string.Format("订单提醒:{0}客人您好,您预订的{1}酒店 {2}房型,商家有事需要与您沟通,请您看到短信后及时与商家旺旺或电话联系,以免耽误您的行程.如有问题请致电{3}", order.contactName, order.hotelName, order.roomName, phone);
                }
                else if (type == 3)    //开票提醒
                {
                    message = string.Format(@"发票开具提醒：您预订的{0}酒店已入住离店，为了避免影响您报销，如需开发票请尽快操作收货并旺旺或电联小店{1} 提供您的开票信息与收货人信息，发票申请需离店后1个月内提交信息，统一安排顺丰到付邮寄（邮费由收件人面付），逾期无法开具，感谢您的配合！", order.hotelName, phone);
                }
                else if (type == 4)
                {
                    message = string.Format(@"尊敬的{0}客人， 你预订的{1}酒店，订单需要你修改一下退款原因协商一致或者行程变更，或者不需要/不想要了，以下是重新修改退款链接，你点击进去即可修改 http://dwz.cn/2Q63ko 祝您旅途愉快！如有问题，请联系旺旺或商家电话: {2} ", order.contactName, order.hotelName, phone);
                }
                else if (type == 5)    //无法联系客人 
                {
                    message = string.Format("尊敬的{0}客人，您预订的{1}酒店，收到您来电告知到店查询不到订单，此单已核实有预定，您可以在前台直接报{2}+18198103603进行查询，不需要报飞猪网，由于无法接通您的手机，在此短信通知您可以正常安排入住，祝您入住愉快！如有问题，请联系我们的旺旺或商家电话:{3} ", order.contactName, order.hotelName, order.sourceOrderID, phone);
                }
                else if (type == 6)    //修改退款原因
                {
                    message = string.Format("尊敬的{0}客人，您预订的{1}酒店，由于订单是部分退款，需要您修改一下退款金额，以及退款原因为协商一致或者行程变更，以下是重新修改退款链接，点击进去即可修改 http://t.cn/RG0yGDm 祝您旅途愉快！如有问题，请联系旺旺或商家电话:{2} ", order.contactName, order.hotelName, phone);
                }


                if (shop == ShopType.ShengLv || shop == ShopType.RenNiXing)
                {
                    message = GetShengLvCommunicationMessage(order, type);
                }

                message = string.Format("{0}{1}", message, shopPrefix);
                bool flag = false;
                var sendResult = string.Empty;
                if (shop == ShopType.ShengLv || shop == ShopType.RenNiXing)
                {
                    sendResult = SMSUilitily.SendShengLv(order.contactTel, message,1);
                    try
                    {
                        var match = Regex.Match(sendResult, "[0-9]{10,20}");
                        flag = Convert.ToInt64(match.Value) > 0;
                    }
                    catch
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = SMSUilitily.Send(1, order.contactTel, message);
                }

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        string sql = string.Format("INSERT INTO [SMSRecord]([Phone],[Content],[Result],[CreateTime])VALUES('{0}','{1}','{2}',getdate())", order.contactTel, message, sendResult);
                        SqlSugarContext.ResellbaseInstance.Ado.ExecuteCommand(sql);
                    }
                    catch
                    {
                    }
                });

                if (flag)
                {
                    string sql = string.Format("INSERT INTO [AliTripSMS]([taobaoOrderId],[Tell],[Content] ,[ShopType],[Source],[CreateTime])VALUES({2},'{0}','{1}',5,5,getdate())", order.contactTel, message, order.taoBaoOrderId);
                    SqlSugarContext.BigTreeInstance.Ado.ExecuteCommand(sql);

                    string caozuo = string.Format("{0} 发送{1}短信 ", username, ((MessageType)type).GetDescription());
                    orderRepository.UpdateOrderCaoZuo(order.aId, caozuo);
                    return result.SetSucess("短信发送成功");
                }
                else
                {
                    return result.SetError("短信发送失败");
                }
            }
            catch (Exception ex)
            {
                return result.SetError("短信发送失败，请重试(如果遇到多次发送失败，请联系相关技术)");
            }
        }


        private static string GetShengLvCommunicationMessage(TB_hotelcashorder order, int type)
        {
            string message = string.Empty;
            string orderNo = string.IsNullOrEmpty(order.sourceOrderID) ? order.taoBaoOrderId.ToString() : order.sourceOrderID;
            if (type == 1)        //满房
            {
                message = string.Format("尊敬的{0}客人,您预订的{1}酒店 订单号：{2} 经酒店回复无法确认订单,为避免影响您的行程，请申请退款并改订其他，祝您旅途愉快！如需帮助请致电0851-88574693 ", order.contactName, order.hotelName, order.taoBaoOrderId);
            }
            else if (type == 2)   //沟通
            {
                message = string.Format("尊敬的{0}客人,您预订的{1}酒店 豪华标准间房型,商家有事需要与您沟通,请您看到短信后及时与商家旺旺或电话联系,以免耽误您的行程，如需帮助请致电0851-88574693 ", order.contactName, order.hotelName);
            }
            else if (type == 3)    //开票提醒
            {
                message = string.Format(@"尊敬的{0}客人，您预订的{1}酒店已入住离店，如需发票请操作收货，并联系商家旺旺或拨打热线0851-88574693提供您的开票相关信息，需离店后1个月内提交发票申请，逾期无法开具，感谢您的配合！", order.contactName, order.hotelName);
            }
            else if (type == 4)   //部分退款
            {
                message = string.Format(@"尊敬的{0}客人,您预订的{1}酒店，订单号{2}经申请可部分退款，您需修改退款金额，退款原因选择协商一致，可戳链接 http://t.cn/RG0yGDm 操作，如需帮助请联系商家旺旺或电话0851-88574693 ", order.contactName, order.hotelName, orderNo);
            }
            else if (type == 5)    //无法联系客人 
            {
                message = string.Format("尊敬的{0}客人，您预订的{1}酒店，收到您来电告知到店查询不到订单，此单已核实有预定，您可以在前台直接报{2}+18198103603进行查询，不需要报飞猪网，由于无法接通您的手机，在此短信通知您可以正常安排入住，祝您入住愉快！如有问题，请联系我们的旺旺或商家电话:0851-88574693 ", order.contactName, order.hotelName, order.sourceOrderID);
            }
            else if (type == 6)    //修改退款原因
            {
                message = string.Format(@"尊敬的{0}客人,您预订的{1}酒店，订单号{2}经申请可部分退款，您需修改退款金额，退款原因选择协商一致，可戳链接 http://t.cn/RG0yGDm 操作，如需帮助请联系商家旺旺或电话0851-88574693 ", order.contactName, order.hotelName, orderNo);
            }
            return message;
        }
        #endregion

    }
}


