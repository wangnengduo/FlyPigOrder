using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using Flypig.Order.Application.Order.Entities;
using FlyPig.Order.Framework.HttpWeb;
using Newtonsoft.Json;
using FlyPig.Order.Framework.Logging;

namespace FlyPig.Order.Application.Order.Notice
{
    /// <summary>
    /// 试单通知
    /// </summary>
    public class ValidateNotice : BaseOrderNotice<ValidateRQ, ValidateRQResult>
    {
        private readonly LogWriter logWriter;
        public ValidateNotice(ShopType shop) : base(shop)
        {
            logWriter = new LogWriter("Tmall/CreateToken");
        }

        protected override string ChannelDiscernmentCode => Request.RatePlanCode;

        protected override ValidateRQResult Notice()
        {
            var result = new ValidateRQResult();
            var hotelInfo = GetIdInfoByRpCode(Request.RatePlanCode);
            int IsCustomer = 1;
            if (Request.AuthenticationToken != null && Request.AuthenticationToken.CreateToken != null && Request.AuthenticationToken.CreateToken.ToLower().Contains("validate"))
            {
                //飞猪程序测试
                IsCustomer = 0;
            }
            //logWriter.Write("记录CreateToken：{0},转为小写：{1}", Request.AuthenticationToken.CreateToken, Request.AuthenticationToken.CreateToken.ToLower());
            var bookCheckDto = new BookingCheckInputDto
            {
                CheckIn = Request.CheckIn,
                CheckOut = Request.CheckOut,
                HotelId = hotelInfo.HotelId,
                RoomTypeId = hotelInfo.RoomTypeId,
                RatePlanId = hotelInfo.RatePlanId,
                RatePlanCode= Request.RatePlanCode,
                Rpid = Request.TaoBaoRatePlanId,
                Hid = Request.TaoBaoHotelId,
                RoomNum = Request.RoomNum,
                OuterId= Request.RoomTypeId,
                CustomerNumber = Request.CustomerNumber,
                IsCustomer = IsCustomer
            };

            if (Request.RatePlanCode.Split('_').Length > 3)
            {
                bookCheckDto.IsVirtual = true;
            }
            #region 
            /*
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string url = string.Empty;
                    if (Channel == ProductChannel.MT)
                    {
                        url = string.Format("http://47.106.239.82/MtPricePro/AliChek.ashx?key=qoeutpzmbjhsyehgixjsngklzoeuyhbgfs&hid=0&code={0}&checkin={1}&checkout={2}&shop=yj&visit=验单失败或下单成功&invoice=1&addplan=0", bookCheckDto.HotelId, bookCheckDto.CheckIn.ToString("yyyy-MM-dd"), bookCheckDto.CheckOut.ToString("yyyy-MM-dd"));

                    }
                    else if (Channel == ProductChannel.Ctrip)
                    {
                        url = string.Format("http://47.106.239.82/CtripPricePro/CtripAliChek.ashx?key=adfiadofdcvjnfgqpofguang&code={0}&checkin={1}&checkout={2}&shop=test&visit=demo", bookCheckDto.HotelId, bookCheckDto.CheckIn.ToString("yyyy-MM-dd"), bookCheckDto.CheckOut.ToString("yyyy-MM-dd"));
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        WebHttpRequest.Get(url);
                    }

                    //if (Shop != ShopType.YinJi)
                    //{
                    //    //异步更新房态
                    //    RoomRateService roomRateService = new RoomRateService(Shop, Channel);
                    //    roomRateService.UpdateRoomRateByHid(hotelInfo.HotelId, true);
                    //}
                }
                catch
                {

                }
            });
            */
            #endregion

            var bookCheckResult = productChannel.BookingCheck(bookCheckDto);
            if (bookCheckResult.IsBook)
            {
                result.ResultCode = "0";
                result.Message = bookCheckResult.Message;
                result.InventoryPrice = JsonConvert.SerializeObject(bookCheckResult.DayPrice);
            }
            else
            {
                result.ResultCode = "-1";
                result.Message = bookCheckResult.Message;
                result.InventoryPrice = string.Empty;

                #region 异步关闭当日房间
                //Task.Factory.StartNew(() =>
                //{
                //    try
                //    {
                //        ////异步更新房态
                //        //RoomRateService roomRateService = new RoomRateService(Shop, Channel);
                //        //roomRateService.CloseRoom(Request.RoomTypeId, Request.RatePlanCode, Request.CheckIn, Request.CheckOut);
                //    }
                //    catch
                //    {
                //    }
                //});
                #endregion
            }

            #region 记录试单信息
            //记录试单信息
            /*
            Task.Factory.StartNew(() =>
            {
                try
                {
                    AliTripValidate av = new AliTripValidate();
                    av.CheckInTime = Request.CheckIn;
                    av.CheckOutTime = Request.CheckOut;
                    av.RatePlanCode = Request.RatePlanCode;
                    av.IsFull = result.ResultCode == "0" ? false : true;
                    av.HotelId = hotelInfo.HotelId;
                    av.RoomId = hotelInfo.RoomTypeId;
                    av.RatePlanId = hotelInfo.RatePlanId;
                    av.CreateTime = DateTime.Now;

                    if (result.Message.Contains("校验成功"))
                    {
                        av.IsFull = false;
                    }
                    av.Channel = (int)Channel;
                    av.Shop = (int)Shop;
                    av.Remark = result.Message;
                    SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();
                }
                catch
                {
                }
            });
            */
            #endregion

            //if (Request.AuthenticationToken.CreateToken.ToLower().Contains("validate"))
            //{
            //    logWriter.Write("记录CreateToken：{0},返回内容：{1}，RatePlanCode:{2}", Request.AuthenticationToken.CreateToken, result.ResultCode, Request.RatePlanCode);
            //}
            logWriter.Write("记录静订结果：请求数据{0},返回内容：{1}", Newtonsoft.Json.JsonConvert.SerializeObject(bookCheckDto), Newtonsoft.Json.JsonConvert.SerializeObject(result));
            return result;
        }
    }
}
