using DaDuShi;
using DaDuShi.Common;
using DaDuShi.Entities;
using FlyPig.Order.Application.Common;
using FlyPig.Order.Application.Common.Tmall;
using FlyPig.Order.Application.Entities.Domestic;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.MT.Entity;
using FlyPig.Order.Application.Repository;
using FlyPig.Order.Application.Repository.Channel;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using FlyPig.Order.Framework.HttpWeb;
using FlyPig.Order.Framework.Logging;
using MeiTuan;
using MeiTuan.Common;
using MeiTuan.Request;
using MeiTuan.Response;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Top.Api.Request;
using Top.Api.Response;
using DaDuShi.Request;
using DaDuShi.Response;

namespace FlyPig.Order.Application.Hotel.Channel
{
    public class DaDuShiProductChannel : IProductChannel
    {
        private readonly LogWriter logWriter;

        private readonly DaDuShiApiClient daDuShiApiClient;
        //private readonly MeiTuanRepository meituanRepository;
        //private readonly HotelRepository hotelRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="channel"></param>
        public DaDuShiProductChannel(ShopType shop, ProductChannel channel)
        {
            Channel = channel;
            Shop = shop;
            DaDuShiConfig config = new DaDuShiConfig();
            if (Shop == ShopType.RenNiXing)
            {
                config = DaDuShiConfigManager.RenNiXing;
            }

            daDuShiApiClient = new DaDuShiApiClient(config);

            logWriter = new LogWriter("Tmall/Validate");
        }

        public ShopType Shop { get; set; }

        public ProductChannel Channel { get; set; }

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

        public BookingCheckOutDto BookingCheck(BookingCheckInputDto checkDto)
        {
            var checkKey = "dds_" + checkDto.HotelId + "_" + checkDto.RoomTypeId + "_" + checkDto.RatePlanId + "_" + checkDto.CheckIn + "_" + checkDto.CheckOut;
            var bookOutDto = new BookingCheckOutDto();
            try
            {
                DateTime startTime = DateTime.Now;
                bookOutDto.DayPrice = new List<RateQuotasPrice>();
                bookOutDto.IsBook = true;
                bookOutDto.Message = "正常可预定";

                int RoomNum = 1;//库存
                //RatePlanConfrimJsonEt RequestEt = new RatePlanConfrimJsonEt();
                var RequestEt = new RatePlanConfrimJsonEt
                {
                    HotelCode = checkDto.HotelId,
                    RatePlanCode = checkDto.RatePlanId,
                    RoomTypeCode = checkDto.RoomTypeId,
                    CheckIn =checkDto.CheckIn.ToString("yyyy-MM-dd"),
                    CheckOut = checkDto.CheckOut.ToString("yyyy-MM-dd"),
                    NumberOfUnits = RoomNum,
                    AdultCount = 2,
                    ChildCount = 0
                };


                ///API_用户认证失败 = 401, API_内部程序出错 = 402,API_订单号已存在 = 403,API_价格不正确 = 404,API_新错误代码 = 405,程序代码出错 = 501,成功 = 200,无数据返回 = 201
                //var response = daDuShiApiClient.RequestData(OperationCode.获取价格确认数据, RequestEt
                //    , () =>
                //    {
                //        return OrderDataOpEt.Availability(RequestEt);
                //    });

                var response = new WebServiceResponse<HotelInfoJsonEt>();
                try
                {
                    response = OrderDataOpEt.Availability(RequestEt);
                    if (response == null || !response.Successful && response.ResponseEt == null || response.ResponseEt.RatePlanList.Count == 0)
                    {
                        RequestEt = new RatePlanConfrimJsonEt
                        {
                            HotelCode = checkDto.HotelId,
                            RatePlanCode = checkDto.RatePlanId,
                            RoomTypeCode = checkDto.RoomTypeId,
                            CheckIn = checkDto.CheckIn.ToString("yyyy-MM-dd"),
                            CheckOut = checkDto.CheckOut.ToString("yyyy-MM-dd"),
                            NumberOfUnits = RoomNum,
                            AdultCount = 1,
                            ChildCount = 0
                        };
                        response = new WebServiceResponse<HotelInfoJsonEt>();
                    }
                }
                catch (Exception ex)
                {
                    response = OrderDataOpEt.Availability(RequestEt);
                }
                TimeSpan ts = checkDto.CheckOut.Subtract(checkDto.CheckIn);
                if (response != null && response.Successful && response.ResponseEt != null && response.ResponseEt.RatePlanList.Count > 0)
                {
                    bookOutDto.IsBook = true;
                    

                    var rate = new TaobaoRate();
                    try
                    {
                        XhotelRateGetRequest req = new XhotelRateGetRequest();
                        req.OutRid = checkDto.OuterId;
                        req.Rpid = checkDto.Rpid;
                        var tmallApiClient = new TmallApiClient(Shop);
                        XhotelRateGetResponse resp = tmallApiClient.Execute(req);
                        if (resp != null && !resp.IsError && resp.Rate != null && !string.IsNullOrWhiteSpace(resp.Rate.InventoryPrice))
                        {
                            rate = JsonConvert.DeserializeObject<TaobaoRate>(resp.Rate.InventoryPrice);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    bool bianhua = false;

                    bookOutDto.DayPrice = response.ResponseEt.RatePlanList[0].RateList.Select(u =>
                        new RateQuotasPrice
                        {
                            Date = u.EffectiveDate,
                            Price = (GetSalePrice(Convert.ToDateTime(u.EffectiveDate),false, u.AmountBeforeTax,rate, ref bianhua, checkDto.IsCustomer)

                            ).ToTaoBaoPrice(),
                            Quota = RoomNum//默认库存
                        }).ToList();
                    
                    //记录试单信息
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            bool IsFull = false;
                            string Remark = bookOutDto.Message;
                            if (checkDto.IsCustomer == 0)
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
                            av.Channel = 10;
                            av.Shop = (int)Shop;
                            av.Remark = Remark;
                            SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();
                            string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source=10", checkDto.HotelId);
                            WebHttpRequest.Get(url);
                        }
                        catch
                        {

                        }
                    });
                }
                else
                {
                    string responseResult = "满房";
                    if (response != null && !response.Successful && !string.IsNullOrWhiteSpace(response.ErrMsg))
                    {
                        responseResult = response.ErrMsg;
                    }
                    try
                    {
                        var falseCount = 1;
                        var falseCacheCount = CacheHelper.GetCache("falseCount");//先读取缓存
                        if (checkDto.IsCustomer == 1)
                        {
                            if (falseCacheCount != null)//如果没有该缓存,默认为1
                            {
                                falseCount = (int)falseCacheCount + 1;
                            }
                            CacheHelper.SetCache("falseCount", falseCount, 1800);//添加缓存
                        }
                        //如果试单失败为双数直接拿缓存值输出，单数时为失败
                        bool tongguo = true;
                        
                        int NowHour = DateTime.Now.Hour;//当前时间的时数
                        int NowMinute = DateTime.Now.Minute;//当前时间的分钟数
                        //if (falseCount % 6 == 0 || checkDto.IsCustomer == 0)
                        if(checkDto.IsCustomer == 0)
                        {
                            bookOutDto.Message = responseResult;
                            bookOutDto.IsBook = false;
                        }
                        else if (falseCount % 4 == 0)
                        {
                            var rate = new TaobaoRate();
                            XhotelRateGetRequest req = new XhotelRateGetRequest();
                            req.OutRid = checkDto.OuterId;
                            req.Rpid = checkDto.Rpid;
                            var tmallApiClient = new TmallApiClient(Shop);
                            XhotelRateGetResponse resp = tmallApiClient.Execute(req);
                            if (resp != null && !resp.IsError && resp.Rate != null && !string.IsNullOrWhiteSpace(resp.Rate.InventoryPrice))
                            {
                                rate = JsonConvert.DeserializeObject<TaobaoRate>(resp.Rate.InventoryPrice);
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
                                    bookOutDto.Message = responseResult;
                                    bookOutDto.IsBook = false;
                                }
                                else
                                {
                                    rates.Add(inventory_price);
                                }
                            }

                            if (IsClose == 0)
                            {
                                bookOutDto.DayPrice = rates.Select(u =>
                                new RateQuotasPrice
                                {
                                    Date = u.date,
                                    Price = u.price,
                                    Quota = 1
                                }).ToList();
                                bookOutDto.Message = "正常可预定";
                                bookOutDto.IsBook = true;
                                logWriter.Write("试单失败后通过（大都市）：试单失败-淘宝酒店id：{0},美团酒店id：{1},{2},失败数次：{3},失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, responseResult);
                            }
                        }
                        else if (falseCount % 3 == 0)
                        {
                            var rate = new TaobaoRate();
                            XhotelRateGetRequest req = new XhotelRateGetRequest();
                            req.OutRid = checkDto.OuterId;
                            req.Rpid = checkDto.Rpid;
                            var tmallApiClient = new TmallApiClient(Shop);
                            XhotelRateGetResponse resp = tmallApiClient.Execute(req);
                            if (resp != null && !resp.IsError && resp.Rate != null && !string.IsNullOrWhiteSpace(resp.Rate.InventoryPrice))
                            {
                                rate = JsonConvert.DeserializeObject<TaobaoRate>(resp.Rate.InventoryPrice);
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
                                    bookOutDto.Message = responseResult;
                                    bookOutDto.IsBook = false;
                                }
                                else
                                {
                                    rates.Add(inventory_price);
                                }
                            }

                            if (IsClose == 0)
                            {
                                Random ran = new Random();
                                int RandomResult = ran.Next(80, 200);

                                bookOutDto.DayPrice = rates.Select(u =>
                                new RateQuotasPrice
                                {
                                    Date = u.date,
                                    Price = u.price + RandomResult * 100m,
                                    Quota = 1
                                }).ToList();
                                bookOutDto.Message = "正常可预定";
                                bookOutDto.IsBook = true;
                                logWriter.Write("试单失败后通过（大都市）：试单失败-淘宝酒店id：{0},美团酒店id：{1},{2},失败数次：{3},失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, responseResult);
                            }
                        }
                        else
                        {
                            logWriter.Write("试单失败后（大都市）：试单失败-淘宝酒店id：{0},美团酒店id：{1},{2},失败数次：{3},失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, responseResult);
                            bookOutDto.Message = responseResult;
                            bookOutDto.IsBook = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        logWriter.Write("试单失败后报错（大都市）：试单失败-淘宝酒店id：{0},美团酒店id：{1},{2},失败原因{3}，报错{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, responseResult, ex.ToString());
                        bookOutDto.Message = responseResult;
                        bookOutDto.IsBook = false;
                    }
                    
                    //记录试单信息并关闭房态
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            bool IsFull = true;
                            string Remark = responseResult;
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

                            av.Channel = 10;
                            av.Shop = (int)Shop;
                            av.Remark = Remark;
                            SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();
                        }
                        catch
                        {
                        }
                        string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source=10", checkDto.HotelId);
                        WebHttpRequest.Get(url);
                    });
                    
                }
            }
            catch (Exception ex)
            {
                bookOutDto.IsBook = false;
                bookOutDto.Message = string.Format("满房，系统异常：{0}", ex.ToString());
            }
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
            CacheHelper.SetCache(checkKey, Newtonsoft.Json.JsonConvert.SerializeObject(bookOutDto), 8);//添加缓存
            return bookOutDto;
        }


        public TmallHotelPriceInfo GetHotelPriceInfo(BookingCheckInputDto checkDto)
        {
            string hotelId = checkDto.HotelId;
            string rpid = checkDto.RatePlanId;

            string ratePlanId = checkDto.RatePlanId;
            string lastTime = "23:59";
            string priceStr = string.Empty;
            decimal totalPrice = 0;

            TmallHotelPriceInfo priceInfo = new TmallHotelPriceInfo();
            priceInfo.BreakFast = 99;
            #region 获取酒店名称
            try
            {
                string sql = string.Format("select top 1 HotelName as HotelName from DaDuShi_Hotels(NOLOCK) where HotelCode = '{0}'", hotelId);
                var getHotelInfo = SqlSugarContext.ResellbaseInstance.SqlQueryable<MeiTuanHotel>(sql).First();
                if (getHotelInfo != null && getHotelInfo.HotelName != null)
                {
                    priceInfo.HotelName = getHotelInfo.HotelName;
                }
                else
                {
                    priceInfo.HotelName = "酒店未获取";
                }
            }
            catch
            {
                priceInfo.HotelName = "酒店未获取";
            }
            #endregion

            var RequestEt = new RatePlanConfrimJsonEt
            {
                HotelCode = checkDto.HotelId,
                RatePlanCode = checkDto.RatePlanId,
                RoomTypeCode = checkDto.RoomTypeId,
                CheckIn = checkDto.CheckIn.ToString("yyyy-MM-dd"),
                CheckOut = checkDto.CheckOut.ToString("yyyy-MM-dd"),
                NumberOfUnits = checkDto.RoomNum,
                AdultCount = 2,
                ChildCount = 0
            };
            //当失败时重新试单
            var response = new WebServiceResponse<HotelInfoJsonEt>();
            try
            {
               response = OrderDataOpEt.Availability(RequestEt);
            }
            catch (Exception exMt)
            {
                response = OrderDataOpEt.Availability(RequestEt);
            }
            #region 默认为2个人请求，当失败时为一个人请求
            if (response == null || !response.Successful || response.ResponseEt == null || response.ResponseEt.RatePlanList.Count == 0)
            {
                RequestEt = new RatePlanConfrimJsonEt
                {
                    HotelCode = checkDto.HotelId,
                    RatePlanCode = checkDto.RatePlanId,
                    RoomTypeCode = checkDto.RoomTypeId,
                    CheckIn = checkDto.CheckIn.ToString("yyyy-MM-dd"),
                    CheckOut = checkDto.CheckOut.ToString("yyyy-MM-dd"),
                    NumberOfUnits = checkDto.RoomNum,
                    AdultCount = 1,
                    ChildCount = 0
                };
                //当失败时重新试单
                response = new WebServiceResponse<HotelInfoJsonEt>();
                try
                {
                    response = OrderDataOpEt.Availability(RequestEt);
                }
                catch (Exception exMt)
                {
                    response = OrderDataOpEt.Availability(RequestEt);
                }
            }
            #endregion

            if (response != null && response.Successful && response.ResponseEt != null && response.ResponseEt.RatePlanList.Count > 0)
            {
                foreach (var item in response.ResponseEt.RatePlanList[0].RateList)
                {
                    var basePrice = item.AmountBeforeTax;
                    priceStr += "price" + basePrice + "|" + basePrice + "|money" + basePrice + "|0|";
                    totalPrice += basePrice;
                }
                priceInfo.DatePrice = totalPrice;
                priceInfo.PriceStr = priceStr;
                priceInfo.RoomName = response.ResponseEt.RatePlanList[0].RoomTypeName;
                priceInfo.RatePlanName = response.ResponseEt.RatePlanList[0].RoomTypeName;
                priceInfo.PaymentType = 1;
                priceInfo.BreakFast = response.ResponseEt.RatePlanList[0].RateList.Min(s => s.Breakfast);
                
            }
            else
            {
                try
                {
                    //当不可预订时获取房型名称失败，重新到数据库获取
                    string sql = string.Format("select top 1 RoomTypeName as HotelName from DaDuShi_RoomType WITH(NOLOCK) where HotelCode = '{0}' and RoomTypeCode='{1}'", hotelId, checkDto.RoomTypeId);
                    var getHotelInfo = SqlSugarContext.ResellbaseInstance.SqlQueryable<MeiTuanHotel>(sql).First();
                    if (getHotelInfo != null && getHotelInfo.HotelName != null)
                    {
                        priceInfo.RoomName = getHotelInfo.HotelName;
                    }
                    else
                    {
                        priceInfo.RoomName = "获取房型失败";
                    }
                }
                catch
                {
                    priceInfo.RoomName = "获取房型失败"; ;
                }
            }

            return priceInfo;
        }

        /// <summary>
        /// 获取销售价格
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="isCommission">是否代理</param>
        /// <param name="guidePrice">指导价</param>
        /// <param name="TaobaoRate">上传给淘宝的价格计划</param>
        /// /// <param name="bianhua">输出价格是否发生变化</param>
        /// <param name="IsCustomer">1为是客户请求，0为程序请求</param>
        ///  bool isVirtual, bool isCommission, int invoiceMode, decimal subRate, decimal guidePrice, AliTripHotelExtension hotelExtension, TaobaoRate rate
        /// <returns></returns>
        private decimal GetSalePrice(DateTime date, bool isCommission, decimal basePrice, TaobaoRate rate, ref bool bianhua, int IsCustomer = 1)
        {
            decimal salePrice = basePrice;
            var nowDate = DateTime.Now;

            salePrice = salePrice * 1.075m;

            bianhua = false;
            if (date >= new DateTime(2019, 12, 31))
            {
                salePrice = salePrice * 1.015m;
            }
            salePrice = Math.Floor(salePrice) + 0.49m;
            if (rate != null && rate.inventory_price != null && rate.inventory_price.Count > 0)
            {
                var inventory_price = rate.inventory_price.Where(a => Convert.ToDateTime(a.date) == date).FirstOrDefault();
                if (inventory_price.price > 0 && inventory_price.price <= 4999900)
                {
                    //当降价了，直接输出之前推送的价格，当为代理产品时在升价
                    if (Convert.ToDecimal(inventory_price.price) / 100 >= salePrice * 0.97m || (salePrice - Convert.ToDecimal(inventory_price.price) / 100) <= 15)
                    {
                        if (Convert.ToDecimal(inventory_price.price) / 100 != salePrice)
                        {
                            bianhua = true;
                        }
                        if (IsCustomer != 0)
                        {
                            return Convert.ToDecimal(inventory_price.price) / 100;
                        }
                    }
                    else
                    {
                        bianhua = true;
                    }
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
