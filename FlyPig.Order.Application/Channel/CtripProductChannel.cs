using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyPig.Order.Application.Entities.Domestic;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Order;
using FlyPig.Order.Application.Repository.Channel;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using FlyPig.Order.Framework.HttpWeb;
using FlyPig.Order.Application.Repository;
using Ctrip.Response;
using Ctrip;
using FlyPig.Order.Application.Common;
using Top.Api.Request;
using Top.Api.Response;
using FlyPig.Order.Application.Common.Tmall;
using Newtonsoft.Json;
using FlyPig.Order.Framework.Logging;
using System.Threading;

namespace FlyPig.Order.Application.Hotel.Channel
{
    public class CtripProductChannel : IProductChannel
    {
        private readonly LogWriter logWriter;

        private CtripApiClient ctripApiClient;
        private readonly HotelRepository hotelRepository;
        private readonly CtripRepository ctripRepository;

        public CtripProductChannel(ShopType shopType)
        {
            ctripApiClient = new CtripApiClient(Ctrip.Config.ApiConfigManager.ChenYi);
            this.Shop = shopType;
            ctripRepository = new CtripRepository();
            hotelRepository = new HotelRepository(ProductChannel.Ctrip);

            logWriter = new LogWriter("Tmall/Validate");
        }

        public ShopType Shop { get; set; }

        /// <summary>
        /// 试单
        /// </summary>
        /// <param name="checkDto"></param>
        /// <returns></returns>
        public BookingCheckOutDto BookingCheck(BookingCheckInputDto checkDto)
        {
            var checkKey = "xc_" + checkDto.HotelId + "_" + checkDto.RoomTypeId + "_" + checkDto.RatePlanId + "_" + checkDto.CheckIn + "_" + checkDto.CheckOut;
            
            bool isPromotion = false;//isPromotion是否为促销产品，true为促销
            if (checkDto.RatePlanCode.Contains("_c"))
            {
                ctripApiClient = new CtripApiClient(Ctrip.Config.ApiConfigManager.ZhiHeC);
                isPromotion = true;
            }
            BookingCheckOutDto bookingCheckOut = new BookingCheckOutDto();
            bookingCheckOut.IsBook = true;
            bookingCheckOut.Message = "正常可预定";
            bookingCheckOut.DayPrice = new List<RateQuotasPrice>();
            //如果查询库存为2时可以预定，折输出库存为2，否则为1
            int RoomNum = checkDto.RoomNum;
            if (RoomNum == 1 && !isPromotion)
            {
                RoomNum = 2;
            }
            try
            {
                string lastTime = "23:59";
                var resp = ctripApiClient.CheckOrderAvail(checkDto.HotelId, "501", checkDto.RatePlanId, RoomNum, 1, checkDto.CheckIn, checkDto.CheckOut, lastTime);

                if (resp != null && resp.AvailabilityStatus == "NoAvailability")
                {
                    if (resp.Error != null && resp.Error.Message.Contains("Invalid number of rooms"))
                    {
                        RoomNum = 1;
                        //Task.Factory.StartNew(() =>
                        //{
                        //    var roomTypeService = new RoomTypeService(Shop, Entities.Enum.ProductChannel.Ctrip);
                        //    roomTypeService.ModifyRoomType(checkDto.HotelId);
                        //});
                        resp = ctripApiClient.CheckOrderAvail(checkDto.HotelId, "501", checkDto.RatePlanId, RoomNum, 1, checkDto.CheckIn, checkDto.CheckOut, lastTime);
                    }
                }

                if (resp != null && resp.AvailabilityStatus == "AvailableForSale")
                {
                    if (resp.RoomStay != null && resp.RoomStay.RoomRates != null)
                    {
                        var rate = new TaobaoRate();
                        try
                        {
                            XhotelRateGetRequest req = new XhotelRateGetRequest();
                            req.OutRid = checkDto.OuterId;
                            req.Rpid = checkDto.Rpid;
                            var tmallApiClient = new TmallApiClient(Shop);
                            XhotelRateGetResponse GetRateResp = tmallApiClient.Execute(req);
                            if (resp != null && !GetRateResp.IsError && GetRateResp.Rate != null && !string.IsNullOrWhiteSpace(GetRateResp.Rate.InventoryPrice))
                            {
                                rate = JsonConvert.DeserializeObject<TaobaoRate>(GetRateResp.Rate.InventoryPrice);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        //var hotelExtension = hotelRepository.GetHotelExtension(checkDto.HotelId, (int)ProductChannel.Ctrip);
                        var roomRates = resp.RoomStay.RoomRates.FirstOrDefault();
                        var isCommission = resp.RoomStay.RatePlans[0].SupplierID > 0;
                        //获取是否为金牌酒店
                        decimal ctripStarRate = 0;
                        //var ctripHotel = hotelRepository.GetCtripHotel(checkDto.HotelId);
                        //金牌
                        //ctripStarRate = ctripHotel.CtripStarRate;
                        //是否发生变价
                        bool bianhua = false;
                        //是否需要马上更新
                        bool isUpdate = false;

                        bookingCheckOut.DayPrice = roomRates.Rates.Select(u =>
                        new RateQuotasPrice
                        {
                            Date = u.EffectiveDate,
                            Price = (GetSalePrice(Convert.ToDateTime(u.EffectiveDate), u.Cost, u.AmountBeforeTax, isCommission, rate, isPromotion,ref bianhua, ref isUpdate, ctripStarRate, checkDto.IsCustomer)

                            ).ToTaoBaoPrice(),
                            Quota = RoomNum//默认库存
                        }).ToList();

                        //记录试单信息
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                bool IsFull = false;
                                string Remark= bookingCheckOut.Message;
                                if (bianhua)
                                {
                                    Remark = "变价";
                                    //如果为促销产品关房处理因为缓存没那么快更新完
                                    //if(isPromotion)
                                    //{
                                    //    IsFull = true;
                                    //}
                                }
                                else if (checkDto.IsCustomer == 0)
                                {
                                    Remark = "程序请求";
                                }
                                AliTripValidate av = new AliTripValidate();
                                av.CheckInTime = checkDto.CheckIn;
                                av.CheckOutTime = checkDto.CheckOut;
                                av.RatePlanCode = checkDto.RatePlanCode;
                                av.IsFull = IsFull;
                                av.HotelId = checkDto.HotelId;
                                av.RoomId = checkDto.RoomTypeId;
                                av.RatePlanId = checkDto.RatePlanId;
                                av.CreateTime = DateTime.Now;

                                av.Channel = 8;
                                av.Shop = (int)Shop;
                                av.Remark = Remark;
                                SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();

                                //发生变价时更新rp
                                //if (bianhua)
                                //{

                                //}
                                //如果发生变价时先更新缓存，然后再更新飞猪报价
                                Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        if (isPromotion)
                                        {
                                            //更新促销价格缓存
                                            string urlCtripPrice = string.Format("http://47.107.101.107:8186/CtripPricePro/CtripPromorion.ashx?key=dfiqergnsdkjdiunqebgaupsdh&code={0}&checkin={1}&checkout={2}&cache=1&account=zhpromotion&opt=update", checkDto.HotelId, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                                            WebHttpRequest.Get(urlCtripPrice);
                                        }
                                        else
                                        {
                                            //更新价格缓存
                                            string urlCtripPrice = string.Format("http://47.107.101.107:8186/CtripPricePro/CtripPriceApi.ashx?key=dfiqergnsdkjdiunqebgaupsdh&code={0}&rids={1}&checkin={2}&checkout={3}&cache=0", checkDto.HotelId, checkDto.RatePlanId, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                                            WebHttpRequest.Get(urlCtripPrice);
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    if (checkDto.IsCustomer != 0 && !isUpdate)
                                    {
                                        Thread.Sleep(13000);
                                    }
                                    string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source=8", checkDto.HotelId);
                                    WebHttpRequest.Get(url);
                                });
                            }
                            catch
                            {

                            }
                        });
                    }
                }
                else
                {
                    try
                    {
                        var falseCount = 1;
                        var falseCacheCount = CacheHelper.GetCache("falseCount");//先读取缓存
                        //记录客户请求失败次数
                        if (checkDto.IsCustomer == 1)
                        {
                            if (falseCacheCount != null)//如果没有该缓存,默认为1
                            {
                                falseCount = (int)falseCacheCount + 1;
                            }
                            CacheHelper.SetCache("falseCount", falseCount, 1800);//添加缓存
                        }

                        bool tongguo = false;

                        //在0点15分到4点为试单失败为双数直接拿缓存值输出，其余的都通过
                        int NowHour = DateTime.Now.Hour;//当前时间的时数
                        int NowMinute = DateTime.Now.Minute;//当前时间的分钟数
                        if (falseCount % 2 == 0)
                        {
                            tongguo = true;
                        }
                        else if ((NowHour == 18 && NowMinute >= 30) || (NowHour < 8 && NowHour >= 19))
                        {
                            if (falseCount % 2 == 0 || falseCount % 5 == 0 || falseCount % 7 == 0)
                            {
                                tongguo = true;
                            }
                        }

                        //如果试单失败为双数直接拿缓存值输出，单数时为失败
                        //if (falseCount % 3 == 0)
                        if (checkDto.IsCustomer == 0)
                        {
                            bookingCheckOut.Message = resp.Error.Message;
                            bookingCheckOut.IsBook = false;
                        }
                        else if (tongguo)
                        {
                            var rate = new TaobaoRate();
                            XhotelRateGetRequest request = new XhotelRateGetRequest();
                            request.OutRid = checkDto.OuterId;
                            request.Rpid = checkDto.Rpid;
                            var tmallApiClient = new TmallApiClient(Shop);
                            XhotelRateGetResponse response = tmallApiClient.Execute(request);
                            if (response != null && !response.IsError && response.Rate != null && !string.IsNullOrWhiteSpace(response.Rate.InventoryPrice))
                            {
                                rate = JsonConvert.DeserializeObject<TaobaoRate>(response.Rate.InventoryPrice);
                                //try
                                //{
                                //    rate = JsonConvert.DeserializeObject<TaobaoRate>(response.Rate.InventoryPrice);
                                //}
                                //catch (Exception ex)
                                //{
                                //}
                            }
                            var totalDayNum = (int)(checkDto.CheckOut.Date - checkDto.CheckIn).TotalDays;
                            var rates = new List<inventory_price>();
                            //判断是否已经更新过飞猪房态，0为没更新过，1为已更新过为不可以预定
                            int IsClose = 0;
                            for (int i = 0; i < totalDayNum; i++)
                            {
                                var inventory_price = rate.inventory_price.Where(a => a.date == checkDto.CheckIn.AddDays(i).ToString("yyyy-MM-dd")).FirstOrDefault();
                                if (inventory_price.price <= 0 || inventory_price.price>4999900)
                                {
                                    IsClose = 1;
                                    bookingCheckOut.Message = resp.Error.Message;
                                    bookingCheckOut.IsBook = false;
                                }
                                else
                                {
                                    rates.Add(inventory_price);
                                }
                            }
                            if (IsClose == 0)
                            {
                                bookingCheckOut.DayPrice = rates.Select(u =>
                                new RateQuotasPrice
                                {
                                    Date = u.date,
                                    Price = u.price,
                                    Quota = 1//checkDto.RoomNum
                                }).ToList();
                                bookingCheckOut.Message = "正常可预定";
                                bookingCheckOut.IsBook = true;
                                //logWriter.Write("试单失败后通过（携程）：试单失败-淘宝酒店id：{0},携程酒店id：{1},{2},失败数次：{3}，失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, resp.Error.Message);
                            }
                        }
                        else if (falseCount % 3 == 0)//涨价处理
                        {
                            var rate = new TaobaoRate();
                            XhotelRateGetRequest request = new XhotelRateGetRequest();
                            request.OutRid = checkDto.OuterId;
                            request.Rpid = checkDto.Rpid;
                            var tmallApiClient = new TmallApiClient(Shop);
                            XhotelRateGetResponse response = tmallApiClient.Execute(request);
                            if (response != null && !response.IsError && response.Rate != null && !string.IsNullOrWhiteSpace(response.Rate.InventoryPrice))
                            {
                                rate = JsonConvert.DeserializeObject<TaobaoRate>(response.Rate.InventoryPrice);
                                //try
                                //{
                                //    rate = JsonConvert.DeserializeObject<TaobaoRate>(response.Rate.InventoryPrice);
                                //}
                                //catch (Exception ex)
                                //{
                                //}
                            }
                            var totalDayNum = (int)(checkDto.CheckOut.Date - checkDto.CheckIn).TotalDays;
                            var rates = new List<inventory_price>();
                            //判断是否已经更新过飞猪房态，0为没更新过，1为已更新过为不可以预定
                            int IsClose = 0;
                            for (int i = 0; i < totalDayNum; i++)
                            {
                                var inventory_price = rate.inventory_price.Where(a => a.date == checkDto.CheckIn.AddDays(i).ToString("yyyy-MM-dd")).FirstOrDefault();
                                if (inventory_price.price <= 0 || inventory_price.price > 4999900)
                                {
                                    IsClose = 1;
                                    bookingCheckOut.Message = resp.Error.Message;
                                    bookingCheckOut.IsBook = false;
                                }
                                else
                                {
                                    rates.Add(inventory_price);
                                }
                            }

                            if (IsClose == 0)
                            {
                                //当满房是生成随机数并加上之前上传给飞猪的
                                Random ran = new Random();
                                int RandomResult = ran.Next(80, 200);

                                bookingCheckOut.DayPrice = rates.Select(u =>
                                new RateQuotasPrice
                                {
                                    Date = u.date,
                                    Price = u.price + RandomResult * 100m,
                                    Quota = 1//checkDto.RoomNum
                                }).ToList();
                                bookingCheckOut.Message = "正常可预定";
                                bookingCheckOut.IsBook = true;
                                //logWriter.Write("试单失败后通过（携程）：试单失败-淘宝酒店id：{0},携程酒店id：{1},{2},失败数次：{3}，失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, resp.Error.Message);
                            }
                        }
                        else
                        {
                            //logWriter.Write("试单失败后（携程）：试单失败-淘宝酒店id：{0},携程酒店id：{1},{2},失败数次：{3}，失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, resp.Error.Message);
                            bookingCheckOut.Message = resp.Error.Message;
                            bookingCheckOut.IsBook = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        //logWriter.Write("试单失败后报错（携程）：试单失败-淘宝酒店id：{0},携程酒店id：{1},{2},失败原因{3}，报错{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, resp.Error.Message, ex.ToString());
                        bookingCheckOut.Message = resp.Error.Message;
                        bookingCheckOut.IsBook = false;
                    }
                    //记录试单信息并关闭房态
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            string Remark = resp.Error.Message;
                            bool IsFull = true;
                            if (checkDto.IsCustomer == 0)
                            {
                                Remark = "程序请求";
                                IsFull = false;
                            }
                            AliTripValidate av = new AliTripValidate();
                            av.CheckInTime = checkDto.CheckIn;
                            av.CheckOutTime = checkDto.CheckOut;
                            av.RatePlanCode = checkDto.RatePlanCode;
                            av.IsFull = IsFull;
                            av.HotelId = checkDto.HotelId;
                            av.RoomId = checkDto.RoomTypeId;
                            av.RatePlanId = checkDto.RatePlanId;
                            av.CreateTime = DateTime.Now;

                            av.Channel = 8;
                            av.Shop = (int)Shop;
                            av.Remark = Remark;
                            SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();

                            Task.Factory.StartNew(() =>
                            {
                                try
                                {
                                    if (isPromotion)
                                    {
                                        //更新促销价格缓存
                                        string urlCtripPrice = string.Format("http://47.107.101.107:8186/CtripPricePro/CtripPromorion.ashx?key=dfiqergnsdkjdiunqebgaupsdh&code={0}&checkin={1}&checkout={2}&cache=1&account=zhpromotion&opt=update", checkDto.HotelId, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                                        WebHttpRequest.Get(urlCtripPrice);
                                    }
                                    else
                                    {
                                        //更新价格缓存
                                        string urlCtripPrice = string.Format("http://47.107.101.107:8186/CtripPricePro/CtripPriceApi.ashx?key=dfiqergnsdkjdiunqebgaupsdh&code={0}&rids={1}&checkin={2}&checkout={3}&cache=0", checkDto.HotelId, checkDto.RatePlanId, DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                                        WebHttpRequest.Get(urlCtripPrice);
                                    }
                                }
                                catch
                                {
                                }
                                if (checkDto.IsCustomer != 0)
                                {
                                    Thread.Sleep(12000);
                                }
                                
                                string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source=8", checkDto.HotelId);
                                WebHttpRequest.Get(url);
                            });

                           
                        }
                        catch
                        {

                        }
                    });
                }
            }
            catch (Exception ex)
            {
                //Task.Factory.StartNew(() =>
                //{
                //    var roomRateService = new RoomRateService(Shop, Entities.Enum.ProductChannel.Ctrip);
                //    roomRateService.UpdateRoomRateByHid(checkDto.HotelId, true, true, true);
                //});
                bookingCheckOut.IsBook = false;
                bookingCheckOut.Message = "出现异常，不可预定";
            }
            //如果是飞猪程序调用先获取8秒内是否有试单
            if (checkDto.IsCustomer == 0)
            {
                try
                {
                    BookingCheckOutDto CheckOut = new BookingCheckOutDto();
                    var check = CacheHelper.GetCache(checkKey);//先读取缓存
                    if (check != null)//如果没有该缓存
                    {
                        CheckOut = Newtonsoft.Json.JsonConvert.DeserializeObject<BookingCheckOutDto>(check.ToString(), new JsonSerializerSettings
                        {
                            Error = delegate (object obj, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                            {
                                args.ErrorContext.Handled = true;
                            }
                        });
                        if (CheckOut != null)
                        {
                            return CheckOut;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            CacheHelper.SetCache(checkKey, Newtonsoft.Json.JsonConvert.SerializeObject(bookingCheckOut), 8);//添加缓存8秒
            return bookingCheckOut;
        }

        /// <summary>
        /// 获取酒店基础信息
        /// </summary>
        /// <param name="checkDto"></param>
        /// <returns></returns>
        public TmallHotelPriceInfo GetHotelPriceInfo(BookingCheckInputDto checkDto)
        {
            TmallHotelPriceInfo priceInfo = new TmallHotelPriceInfo();
            if (checkDto.RatePlanCode.Contains("_c"))
            {
                ctripApiClient = new CtripApiClient(Ctrip.Config.ApiConfigManager.ZhiHeC);
            }
            try
            {
                string ratePlanId = checkDto.RatePlanId;
                string lastTime = "23:59";
                string priceStr = string.Empty;
                decimal totalPrice = 0;

                var getHotelInfo = Task.Factory.StartNew(() => { return ctripRepository.GetHotel(checkDto.HotelId); });
                var hotel = getHotelInfo.Result;
                if (hotel == null || hotel.HotelName == null || hotel.HotelName == "")
                {
                    priceInfo.HotelName = "获取酒店失败";
                }
                else
                {
                    priceInfo.HotelName = hotel.HotelName;
                }
                HotelAvailReponse resp = new HotelAvailReponse();
                //当失败时重新试单
                try
                {
                    resp = ctripApiClient.CheckOrderAvail(checkDto.HotelId, "501", ratePlanId, checkDto.RoomNum, 1, checkDto.CheckIn, checkDto.CheckOut, lastTime);
                }
                catch
                {
                    resp = ctripApiClient.CheckOrderAvail(checkDto.HotelId, "501", ratePlanId, checkDto.RoomNum, 1, checkDto.CheckIn, checkDto.CheckOut, lastTime);
                }
                if (resp != null && resp.AvailabilityStatus == "NoAvailability")
                {
                    if (resp.Error != null && resp.Error.Message.Contains("Invalid number of rooms"))
                    {
                        //Task.Factory.StartNew(() =>
                        //{
                        //    var roomTypeService = new RoomTypeService(Shop, Entities.Enum.ProductChannel.Ctrip);
                        //    roomTypeService.ModifyRoomType(checkDto.HotelId);
                        //});
                        try
                        {
                            resp = ctripApiClient.CheckOrderAvail(checkDto.HotelId, "501", ratePlanId, checkDto.RoomNum, 1, checkDto.CheckIn, checkDto.CheckOut, lastTime);
                        }
                        catch
                        {
                            resp = ctripApiClient.CheckOrderAvail(checkDto.HotelId, "501", ratePlanId, checkDto.RoomNum, 1, checkDto.CheckIn, checkDto.CheckOut, lastTime);
                        }
                    }
                }

                if (resp != null && resp.AvailabilityStatus == "AvailableForSale")
                {
                    if (resp.RoomStay != null && resp.RoomStay.RatePlans != null && resp.RoomStay.RoomRates != null)
                    {
                        var ratePlan = resp.RoomStay.RatePlans.FirstOrDefault();
                        var roomRates = resp.RoomStay.RoomRates.FirstOrDefault();
                        // var roomType = resp.RoomStay.RoomTypes.FirstOrDefault();
                        if (roomRates != null || roomRates.Rates != null || roomRates.Rates.Count > 0)
                        {
                            string ExtensionName = string.Empty;
                            try
                            {
                                var ctripHotel = hotelRepository.GetCtripHotel(checkDto.HotelId);
                                //var hotelExtension = SqlSugarContext.LingZhongInstance.Queryable<AliTripHotelExtension>().Where(u => u.HotelId == checkDto.HotelId && u.Source == (int)ProductChannel.Ctrip).First();
                                if (ctripHotel != null && ctripHotel.CtripStarRate != null && ctripHotel.CtripStarRate >= 5)
                                {
                                    ExtensionName = "[金牌]";
                                }
                            }
                            catch
                            {
                            }
                            foreach (var item in roomRates.Rates)
                            {
                                var basePrice = item.AmountBeforeTax;
                                if (item.Cost > 0)
                                {
                                    basePrice = item.Cost;
                                }
                                //var basePrice = item.AmountBeforeTax;
                                priceStr += "price" + basePrice + "|" + basePrice + "|money" + basePrice + "|0|";
                                totalPrice += basePrice;
                            }

                            string RatePlanName = ratePlan.RatePlanName;

                            int invoiceMode = ratePlan.InvoiceTargetType;
                            if (invoiceMode == 1)
                            {
                                RatePlanName = string.Format("{0}[携程开具发票]", RatePlanName);
                            }
                            else if (invoiceMode == 2)
                            {
                                RatePlanName = string.Format("{0}[酒店开具发票]", RatePlanName);
                            }

                            if (ratePlan.SupplierID > 0)
                            {
                                RatePlanName = string.Format("{0}[代理]", RatePlanName);
                            }

                            if (checkDto.RatePlanCode.Contains("_c"))
                            {
                                RatePlanName = string.Format("{0}[促销]", RatePlanName);
                            }


                            priceInfo.DatePrice = totalPrice;
                            priceInfo.PriceStr = priceStr;
                            priceInfo.RoomName = ratePlan.RatePlanName;
                            priceInfo.PaymentType = 1;
                            priceInfo.RatePlanName = RatePlanName + ExtensionName;



                            return priceInfo;
                        }
                    }
                }
                else
                {
                    var getRoomInfo = Task.Factory.StartNew(() => { return ctripRepository.GetRoomType(checkDto.HotelId, checkDto.RoomTypeId); });

                    var roomInfo = getRoomInfo.Result;
                    if (roomInfo == null || roomInfo.RoomName == null || roomInfo.RoomName =="")
                    {
                        priceInfo.RoomName = "获取房型失败";
                    }
                    else
                    {
                        priceInfo.RoomName = roomInfo.RoomName;
                    }
                }
            }
            catch
            {
            }

            return priceInfo;
        }

        /// <summary>
        /// 获取销售价
        /// </summary>
        /// <param name="date"></param>
        /// <param name="basePrice">低价</param>
        /// <param name="guidePrice">指导价</param>
        /// <param name="isCommission">是否为代理</param>
        /// <param name="hotelExtension"></param>
        /// <param name="isPromotion">是否为促销产品</param>
        /// <param name="bianhua">输出价格是否发生变化</param>
        /// <param name="isUpdate">是否需要马上更新</param>
        /// <param name="IsCustomer">1为是客户请求，0为程序请求</param>
        /// <returns></returns>
        private decimal GetSalePrice(DateTime date, decimal basePrice,decimal guidePrice, bool isCommission, TaobaoRate rate,bool isPromotion, ref bool bianhua, ref bool isUpdate, decimal ? hotelExtension=0,int IsCustomer=1)
        {
            bianhua = false;
            isUpdate = true;
            bool isGuidePrice = false;
            bool isAddPrice = false;
            bool isAddRedEnvelope = true;
            //if (hotelExtension != null)
            //{
            //    isGuidePrice = hotelExtension.Extension3 == 1;
            //    isAddPrice = hotelExtension.Extension ?? false;
            //    isAddRedEnvelope = hotelExtension.Extension5 == 1;
            //}
            decimal salePrice = basePrice;
            if (salePrice != 0)
            {
                ////促销产品
                //if (isPromotion)
                //{
                //    salePrice = basePrice * 1.06m + 1m;
                //}
                //else
                //{
                //    salePrice = basePrice * 1.062m + 1;
                //}
                salePrice = basePrice * 1.065m;
                //salePrice = basePrice * 1.065m;
            }
            else
            {
                salePrice = guidePrice;
            }
            if (salePrice < 130)
            {
                salePrice = salePrice + 1;
            }
            decimal ctripSalePrice = salePrice;
            #region
            /*
            #region 接口获取matt那边的卖价，获取不到则返回指导价 [190404]
            try
            {
                string url = string.Format("http://119.23.44.20:9001/YjAliTrip/GetSalePrice?Channel=8&CurDate={0}&GuidePrice={1}&SubRatio=0&IsAgent={4}&IsVirtual=false&IsMedalHotel={2}&IsHzHotel={3}", date.ToString("yyyy-MM-dd"), ctripSalePrice, isAddPrice, !isAddRedEnvelope, isCommission);
                //单位分
                var mattSalePrice = Convert.ToInt32(WebHttpRequest.Get(url));
                if (mattSalePrice > 0)
                {
                    salePrice = mattSalePrice / 100m;
                    return salePrice;
                }
            }
            catch (Exception ex)
            {
            }
            #endregion

            if (isAddPrice)
            {
                //金牌加3个点
                salePrice = salePrice * 1.03m;
            }
            else
            {
                //携程指导价低1个点-1  四舍五入后加0.49
                salePrice = salePrice * 0.99m - 1;
            }

            //if (isCommission && date >= new DateTime(2019, 4, 29))
            //{
            //    salePrice = salePrice * 1.01m;
            //}

            //统一调低0.6个点
            salePrice = salePrice * 0.994m;
            salePrice = Math.Round(salePrice) + 0.49m;
            salePrice = salePrice + 20;
            salePrice = Math.Round(salePrice, 0) + 0.49m;
            */
            #endregion

            #region
            //if (Shop == ShopType.RenNiXing)
            //{
            //    if (hotelExtension != null && hotelExtension >= 5)
            //    {
            //        //金牌加6个点
            //        salePrice = salePrice * 1.06m;
            //    }
            //    //else
            //    //{
            //    //    if (salePrice > 100 && salePrice < 200)
            //    //    {
            //    //        salePrice = salePrice + 1;
            //    //    }
            //    //    else if (salePrice >= 200 && salePrice < 300)
            //    //    {
            //    //        salePrice = salePrice + 2;
            //    //    }
            //    //    else if (salePrice >= 300 && salePrice < 900)
            //    //    {
            //    //        salePrice = salePrice + 3;
            //    //    }
            //    //}
            //}
            #endregion

            //促销产品加1个点
            //if (isPromotion)
            //{
            //    salePrice = salePrice * 1.01M;
            //}
            //代理加一个点
            if (date >= new DateTime(2019, 12, 31))
            {
                salePrice = salePrice * 1.015m;
            }
            if (isCommission)
            {
                salePrice = salePrice * 1.01M;
            }
            salePrice = Math.Floor(salePrice) + 0.49m;
            if (rate != null && rate.inventory_price != null && rate.inventory_price.Count > 0)
            {
                var inventory_price = rate.inventory_price.Where(a => Convert.ToDateTime(a.date) == date).FirstOrDefault();
                if (inventory_price.price > 0 && inventory_price.price <= 4999900)
                {
                    //当降价了，直接输出之前推送的价格，当为代理产品时在升价
                    if (isCommission)
                    {
                        //if (Convert.ToDecimal(inventory_price.price) / 100 >= salePrice * 0.97m || (salePrice - Convert.ToDecimal(inventory_price.price) / 100) <= 35)
                        if (Convert.ToDecimal(inventory_price.price) / 100 >= salePrice * 0.97m || (salePrice - Convert.ToDecimal(inventory_price.price) / 100) <= 5)
                        {
                            if (Convert.ToDecimal(inventory_price.price) / 100 != salePrice)
                            {
                                bianhua = true;
                            }
                            if (IsCustomer != 0)
                            {
                                isUpdate = false;
                                return Convert.ToDecimal(inventory_price.price) / 100;
                            }
                        }
                    }
                    else if (Convert.ToDecimal(inventory_price.price) / 100 >= salePrice * 0.975m || (salePrice - Convert.ToDecimal(inventory_price.price) / 100) <= 5)
                    {
                        if (Convert.ToDecimal(inventory_price.price) / 100 != salePrice)
                        {
                            bianhua = true;
                        }
                        if (IsCustomer != 0)
                        {
                            isUpdate = false;
                            return Convert.ToDecimal(inventory_price.price) / 100;
                        }
                    }
                    else
                    {
                        //判断价格是否发生变化
                        bianhua = true;
                    }
                    //else if ((inventory_price.price / 100) * 1.01m > salePrice)
                    //{
                    //    return inventory_price.price / 100;
                    //}
                }
            }
            return salePrice;
        }

        private class TaobaoRate
        {
            public List<inventory_price> inventory_price { get; set; }
        }
        private class inventory_price
        {
            public int quota { get; set; }
            public int price { get; set; }
            public string date { get; set; }
        }
    }
}
