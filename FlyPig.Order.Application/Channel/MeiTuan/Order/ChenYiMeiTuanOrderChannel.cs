using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order.Message;
using FlyPig.Order.Application.Channel.Ctrip.ctripPriceCacheEntity;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Repository.Order;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using FlyPig.Order.Framework.HttpWeb;
using FlyPig.Order.Framework.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.MT.Order.Channel
{
    public class ChenYiMeiTuanOrderChannel : TmallMeiTuanOrderChannel, IMeiTuanOrderChannel
    {
        private readonly LogWriter logWriter;
        public ChenYiMeiTuanOrderChannel() : this(ShopType.RenNiXing)
        {
            logWriter = new LogWriter("Tmall/Breakfast");
        }

        public ChenYiMeiTuanOrderChannel(ShopType shop) : base(shop)
        {
            logWriter = new LogWriter("Tmall/Breakfast");
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
                    //<br/>
                    string remark = string.Format("【系统】:下单到携程失败（已售完或扣减库存失败） [{0}]", DateTime.Now.ToString());
                    orderRepository.UpdateRemarkState(aid, 63, remark);

                    int NowHour = DateTime.Now.Hour;//当前时间的时数
                    int NowMinute = DateTime.Now.Minute;//当前时间的分钟数
                    FlyPigMessageUility.ThirdCommunication(Shop, Convert.ToInt32(order.aId), "系统", 1);
                    //晚上12点半后，8点前满房时自动发送短信
                    //if ((NowHour == 0  && NowMinute > 30) || (NowHour > 0 && NowHour < 8))
                    //{
                    //    Task.Factory.StartNew(() =>
                    //    {
                    //        try
                    //        {
                    //            //发满房短信（发送满房信息）
                    //            FlyPigMessageUility.ThirdCommunication(Shop, Convert.ToInt32(order.aId), "系统", 1);
                    //        }
                    //        catch
                    //        {

                    //        }
                    //    });
                    //}
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
                int tBreakfast = getBreakfast(order.hotelID, order.roomID, order.ratePlanID,order.orderNO);
                //logWriter.Write("携程：酒店id：{0},roomID：{1},ratePlanID：{2}，早餐数：{3}，飞猪早餐数：{4}，订单号：{5}", order.hotelID, order.roomID, order.ratePlanID, tBreakfast, order.userID,order.orderNO);
                //判断早餐数是否对等
                if (order.userID > tBreakfast && tBreakfast != 99 && order.userID != 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            //当早餐数不对等时更新rp
                            string urlUpdateRp = string.Format("http://localhost:9092/ashx/rnxUpdateRaPlan.ashx?type=rp&source=8&hid={0}", order.hotelID);
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
                    logWriter.Write("携程(供应商早餐数发生变化)：飞猪酒店id：{0}，酒店id：{1},roomID：{2},ratePlanID：{3}，早餐数：{4}，飞猪早餐数：{5}，订单号：{6}", order.TaoBaoHotelId, order.hotelID, order.roomID, order.ratePlanID, tBreakfast, order.userID, order.orderNO);
                    return result.SetError("该订单早餐个数不相等");
                }
                else if(order.userID < tBreakfast)
                {
                    //else if((order.userID < tBreakfast && order.userID != 0) || tBreakfast == 99)
                    //如果供应商的早餐数大于平台的，更新rp
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            string urlUpdateRp = string.Format("http://localhost:9092/ashx/rnxUpdateRaPlan.ashx?type=rp&source=8&hid={0}", order.hotelID);
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
                    logWriter.Write("携程(平台早餐数少)：飞猪酒店id:{0}，酒店id：{1},roomID：{2},ratePlanID：{3}，早餐数：{4}，飞猪早餐数：{5}，订单号：{6}", order.TaoBaoHotelId, order.hotelID, order.roomID, order.ratePlanID, tBreakfast, order.userID, order.orderNO);
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
                din.true_price_str = order.DailyInfoPrice;

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
                    din = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.fax == order.taoBaoOrderId.ToString()).First();
                    if (din != null)
                    {
                        order.state = 3;
                        var excuteRow = SqlSugarContext.ResellbaseInstance.Updateable(order).ExecuteCommand();
                        return result.SetSucess("同步订单成功,订单号({0})", orderId);
                    }
                    //string sql = string.Format("delete dingdan_info where dingdan_num='{0}' ", orderId);
                    //SqlSugarContext.TravelskyInstance.Ado.ExecuteCommand(sql);
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
                dingdan_info din = SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.dingdan_num == orderId).First();
                if (din != null)
                {
                    var order = SqlSugarContext.ResellbaseInstance.Queryable<TB_hotelcashorder>().Where(u => u.aId == aid).First();
                    order.state = 3;
                    var excuteRow = SqlSugarContext.ResellbaseInstance.Updateable(order).ExecuteCommand();
                    return result.SetSucess("同步订单成功,订单号({0})", orderId);
                }
                //string sql = string.Format("delete dingdan_info where dingdan_num='{0}' ", orderId);
                //SqlSugarContext.TravelskyInstance.Ado.ExecuteCommand(sql);
                return result.SetError("同步订单到数据库失败，出现异常：{0}", ex.Message);
            }
        }



        public override bool UpdateCancelRemark(long taobaoOrderId)
        {
            return new ShengLvOrderRepository().UpdateCancelRemark(taobaoOrderId);
        }


        /// <summary>
        /// 获取早餐数
        /// </summary>
        /// <param name="hotelCode"></param>
        /// <param name="RoomTypeId"></param>
        /// <param name="RatePlanId"></param>
        /// <returns></returns>
        public int getBreakfast(string hotelCode, string RoomTypeId, string RatePlanId,string orderNO)
        {
            int Breakfast = 0;
            try
            {
                string rrJson = string.Empty;
                string sql = string.Format(@"SELECT top 1  cache as RoomName FROM dbo.ctripPriceCache(NOLOCK) WHERE hotelCode ='{0}'", hotelCode);
                //现在价格缓存分两台服务器，hotelcode%2=0  在235的数据库，hotelcode%2=1 在107数据库
                if (Convert.ToInt64(hotelCode) % 2 == 0)
                {
                    var rrJsonObj = SqlSugarContext.connectionString235.SqlQueryable<CtripRoomType>(sql).First();
                    if (!string.IsNullOrEmpty(rrJsonObj.RoomName))
                    {
                        rrJson = rrJsonObj.RoomName;
                    }
                }
                else
                {
                    var rrJsonObj = SqlSugarContext.connectionString107.SqlQueryable<CtripRoomType>(sql).First();
                    if (!string.IsNullOrEmpty(rrJsonObj.RoomName))
                    {
                        rrJson = rrJsonObj.RoomName;
                    }
                }

                if (!string.IsNullOrEmpty(rrJson))
                {
                    if (rrJson.IndexOf("[{") >= 0 || rrJson.IndexOf("[]") >= 0)
                    {
                        //原文反序列化
                    }
                    else
                    {
                        //解压后再反序列化
                        rrJson = Decompress(rrJson);
                    }
                    var rrsCache = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CtStsRoomPriceItems>>(rrJson);
                    var RoomTypeInfo = rrsCache.Where(a => a.RoomID == RatePlanId).FirstOrDefault();
                    Breakfast = RoomTypeInfo.RoomPriceInfos[0].MealInfo.NumberOfBreakfast <= 0 ? 0 : RoomTypeInfo.RoomPriceInfos[0].MealInfo.NumberOfBreakfast;
                }

            }
            catch (Exception ex)
            {
                logWriter.Write("携程(ERR)：酒店id：{0},roomID：{1},ratePlanID：{2}，订单号：{3}，报错原因：{4}", hotelCode, RoomTypeId, RatePlanId,orderNO,ex.ToString());
                Breakfast = 99;
            }
            return Breakfast;
        }

        /// <summary>
        /// 解压字符
        /// </summary>
        public static string Decompress(string s)
        {
            byte[] inputBytes = Convert.FromBase64String(s);
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (System.IO.Compression.GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        string ssss = Encoding.UTF8.GetString(outStream.ToArray());
                        return ssss;
                    }
                }
            }
        }
    }
}
