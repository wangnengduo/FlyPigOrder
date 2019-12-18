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

namespace FlyPig.Order.Application.Hotel.Channel
{

    public class MeiTuanProductChannel : IProductChannel
    {
        private readonly LogWriter logWriter;

        private readonly MeiTuanApiClient meiTuanApiClient;
        private readonly MeiTuanRepository meituanRepository;
        private readonly HotelRepository hotelRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="channel"></param>
        public MeiTuanProductChannel(ShopType shop, ProductChannel channel)
        {
            Channel = channel;
            Shop = shop;
            meituanRepository = new MeiTuanRepository();
            hotelRepository = new HotelRepository(channel);
            MeiTuanConfig config = new MeiTuanConfig();
            if (Shop == ShopType.LingZhong || Shop == ShopType.YinJi || Shop == ShopType.RenXing)
            {
                config = MeiTuanConfigManager.XiWan;
            }
            else if (Shop == ShopType.ShengLv)
            {
                config = MeiTuanConfigManager.ShengLv;
            }
            else if (Shop == ShopType.RenNiXing)
            {
                config = MeiTuanConfigManager.RenNiXing;
            }

            meiTuanApiClient = new MeiTuanApiClient(config);

            logWriter = new LogWriter("Tmall/Validate");
        }

        public ShopType Shop { get; set; }

        public ProductChannel Channel { get; set; }

        /// <summary>
        /// 试单
        /// </summary>
        /// <param name="checkDto"></param>
        /// <returns></returns>
        public BookingCheckOutDto BookingCheck(BookingCheckInputDto checkDto)
        {
            var checkKey = "mt_" + checkDto.HotelId + "_" + checkDto.RoomTypeId + "_" + checkDto.RatePlanId + "_" + checkDto.CheckIn + "_" + checkDto.CheckOut;

            var bookOutDto = new BookingCheckOutDto();
            try
            {
                DateTime startTime = DateTime.Now;
                bookOutDto.DayPrice = new List<RateQuotasPrice>();
                bookOutDto.IsBook = true;
                bookOutDto.Message = "正常可预定";

                var response = new BookingCheckResponse();

                int totalPrice = Convert.ToInt32(getTotaPrice(checkDto));
                var request = new BookingCheckRequest
                {
                    checkinDate = checkDto.CheckIn.ToString("yyyy-MM-dd"),
                    checkoutDate = checkDto.CheckOut.ToString("yyyy-MM-dd"),
                    HotelId = checkDto.HotelId,
                    GoodsId = checkDto.RatePlanId,
                    RoomNum = checkDto.RoomNum,
                    totalPrice = totalPrice * checkDto.RoomNum
                };
                if (totalPrice != 0)
                {
                    response = meiTuanApiClient.Excute(request);
                }
                TimeSpan ts = checkDto.CheckOut.Subtract(checkDto.CheckIn);

                if (response != null && response.Result.code == 0 && (response.Result.desc == null || !response.Result.desc.Contains("价格发生变化")))
                {
                    //var hotelExtension = hotelRepository.GetHotelExtension(checkDto.HotelId, (int)Channel);
                    //var invoiceInfo = GetRatePlanInvoice(checkDto.HotelId, checkDto.RatePlanId, checkDto.CheckIn, checkDto.CheckOut);
                    bookOutDto.IsBook = true;
                    int remainRoomNum = response.Result.remainRoomNum;
                    int Quota = 5;
                    if (remainRoomNum < 1)
                    {
                        Quota = 1;
                    }
                    else if (remainRoomNum > 5)
                    {
                        Quota = 5;
                    }
                    else
                    {
                        Quota = remainRoomNum;
                    }

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

                    List<PriceModelsItem> modelList = new List<PriceModelsItem>();

                    foreach (var item in response.Result.priceModels)
                    {
                        if (!modelList.Any(u => u.date == item.date))
                        {
                            modelList.Add(item);
                        }
                    }
                    //是否需要马上更新
                    bool isUpdate = false;
                    //是否供应商发生变价
                    bool bianhua = false;

                    foreach (var item in modelList)
                    {
                        RateQuotasPrice rqp = new RateQuotasPrice();
                        rqp.Date = item.date;
                        //checkDto.IsVirtual, invoiceInfo.IsCommission, invoiceInfo.InvoiceMode, Convert.ToDecimal(item.subRatio / 10000), item.salePrice / 100, hotelExtension, rate
                        rqp.Price = GetSalePrice(Convert.ToDateTime(item.date), checkDto.IsVirtual,  Convert.ToDecimal(item.subRatio / 10000), item.salePrice / 100, rate, ref bianhua, ref isUpdate, checkDto.IsCustomer).ToTaoBaoPrice();
                        rqp.Quota = Quota;//5;
                        bookOutDto.DayPrice.Add(rqp);
                    }
                    //记录试单信息
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            bool IsFull = false;
                            string Remark = bookOutDto.Message;
                            //当库存为0时关房
                            if (remainRoomNum == 0 && checkDto.IsCustomer == 1)
                            {
                                IsFull = true;
                            }
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
                            av.Channel = 5;
                            av.Shop = (int)Shop;
                            av.Remark = Remark;
                            SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();
                            string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source=5", checkDto.HotelId);
                            if (!isUpdate)
                            {
                                Thread.Sleep(15000);
                            }
                            WebHttpRequest.Get(url);
                            //当价格改变时或库存为0时更新房态
                            //if (bianhua || remainRoomNum == 0)
                            //{
                            //    string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source=5", checkDto.HotelId);
                            //    WebHttpRequest.Get(url);
                            //}
                        }
                        catch
                        {

                        }
                        
                    });
                }
                else
                {
                
                    
                        //TODO 满房变价，全部关房..
                        //bookOutDto.IsBook = false;
                        //bookOutDto.Message = response.Result.desc;
                    
                    try
                    {
                        var falseCount = 1;
                        var falseCacheCount = CacheHelper.GetCache("falseCount");//先读取缓存
                        //当为客人请求才做记录数次
                        if (checkDto.IsCustomer == 1)
                        {
                            if (falseCacheCount != null)//如果没有该缓存,默认为1
                            {
                                falseCount = (int)falseCacheCount + 1;
                            }
                            CacheHelper.SetCache("falseCount", falseCount, 1800);//添加缓存
                        }
                        //如果试单失败为双数直接拿缓存值输出，单数时为失败
                        //if (falseCount % 2 == 0)
                        bool tongguo = false;

                        //在0点15分到4点为试单失败为双数直接拿缓存值输出，其余的都通过
                        int NowHour = DateTime.Now.Hour;//当前时间的时数
                        int NowMinute = DateTime.Now.Minute;//当前时间的分钟数
                        if (falseCount % 2 == 0)
                        {
                            tongguo = true;
                        }
                        else if ((NowHour == 18 && NowMinute > 30) || (NowHour >= 19 && NowHour < 8))
                        {
                            if (falseCount % 2 == 0 || falseCount % 5 == 0)
                            {
                                tongguo = true;
                            }
                        }
                        //if (falseCount % 6 == 0 || checkDto.IsCustomer == 0 || response.Result.desc.Contains("价格发生变化"))
                        if (checkDto.IsCustomer == 0)
                        {
                            bookOutDto.Message = response.Result.desc;
                            bookOutDto.IsBook = false;
                        }
                        else if(tongguo || response.Result.desc.Contains("价格发生变化"))
                        {
                            var rate = new TaobaoRate();
                            XhotelRateGetRequest req = new XhotelRateGetRequest();
                            req.OutRid = checkDto.OuterId;
                            req.Rpid = checkDto.Rpid;
                            var tmallApiClient = new TmallApiClient(Shop);
                            XhotelRateGetResponse resp = tmallApiClient.Execute(req);
                            if (resp != null && !resp.IsError && resp.Rate != null && !string.IsNullOrWhiteSpace(resp.Rate.InventoryPrice))
                            {
                                //try
                                //{
                                //    rate = JsonConvert.DeserializeObject<TaobaoRate>(resp.Rate.InventoryPrice);
                                //}
                                //catch (Exception ex)
                                //{
                                //}
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
                                    bookOutDto.Message = response.Result.desc;
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
                                //logWriter.Write("试单失败后通过（美团）：试单失败-淘宝酒店id：{0},美团酒店id：{1},{2},失败数次：{3},失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, response.Result.desc);
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
                                //try
                                //{
                                //    rate = JsonConvert.DeserializeObject<TaobaoRate>(resp.Rate.InventoryPrice);
                                //}
                                //catch (Exception ex)
                                //{
                                //}
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
                                    bookOutDto.Message = response.Result.desc;
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
                                //logWriter.Write("试单失败后通过（美团）：试单失败-淘宝酒店id：{0},美团酒店id：{1},{2},失败数次：{3},失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, response.Result.desc);
                            }
                        }
                        else
                        {
                            //logWriter.Write("试单失败后（美团）：试单失败-淘宝酒店id：{0},美团酒店id：{1},{2},失败数次：{3},失败原因{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, falseCount, response.Result.desc);
                            bookOutDto.Message = response.Result.desc;
                            bookOutDto.IsBook = false;
                        }
                        //记录试单信息并关闭房态
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                string Remark= response.Result.desc;
                                bool IsFull = true;
                                if (checkDto.IsCustomer == 0)
                                {
                                    Remark = "程序请求";
                                    IsFull = false;
                                }
                                else if (response.Result.desc.Contains("价格发生变化"))
                                {
                                    Remark = "价格发生变化";
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

                                av.Channel = 5;
                                av.Shop = (int)Shop;
                                av.Remark = Remark;
                                SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();
                            }
                            catch
                            {
                            }
                            if (checkDto.IsCustomer != 0)
                            {
                                Thread.Sleep(15000);
                            }
                            string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source=5", checkDto.HotelId);
                            WebHttpRequest.Get(url);
                        });
                    }
                    catch (Exception ex)
                    {
                        //记录试单信息并关闭房态
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                bool IsFull = true;
                                string Remark = response.Result.desc;
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

                                av.Channel = 5;
                                av.Shop = (int)Shop;
                                av.Remark = Remark;
                                SqlSugarContext.RenNiXingInstance.Insertable(av).ExecuteCommand();
                            }
                            catch
                            {
                            }
                            Thread.Sleep(6000);
                            string url = string.Format("http://localhost:8097/apiAshx/UpdateRoomRate.ashx?type=RoomRate&hid={0}&source=5", checkDto.HotelId);
                            WebHttpRequest.Get(url);
                        });
                        //logWriter.Write("试单失败后报错（美团）：试单失败-淘宝酒店id：{0},美团酒店id：{1},{2},失败原因{3}，报错{4}", checkDto.Hid, checkDto.HotelId, checkDto.Rpid, response.Result.desc, ex.ToString());
                        bookOutDto.Message = response.Result.desc;
                        bookOutDto.IsBook = false;
                    }
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


        /// <summary>
        /// 获取销售价格
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="isVirtual">是否虚拟价格</param>
        /// <param name="isCommission">是否代理</param>
        /// <param name="invoiceMode">发票模式</param>
        /// <param name="subRate">佣金率</param>
        /// <param name="guidePrice">指导价</param>
        /// <param name="hotelExtension">酒店扩展信息</param>
        /// <param name="TaobaoRate">上传给淘宝的价格计划</param>
        /// <param name="bianhua">输出价格是否发生变化</param>
        /// <param name="IsCustomer">1为是客户请求，0为程序请求</param>
        ///  bool isVirtual, bool isCommission, int invoiceMode, decimal subRate, decimal guidePrice, AliTripHotelExtension hotelExtension, TaobaoRate rate
        /// <returns></returns>
        private decimal GetSalePrice(DateTime date, bool isVirtual, decimal subRate, decimal guidePrice, TaobaoRate rate, ref bool bianhua, ref bool isUpdate,int IsCustomer = 1)
        {
            bianhua = false;
            isUpdate = true;
            decimal salePrice = guidePrice;
            decimal addPoint = 1;
            decimal basePrice = guidePrice * (1 - (decimal)subRate);
            var nowDate = DateTime.Now;
            var isGuidePrice = 0;
            var isReduce = false;
            var deductPoint = false;
            var isAddRedEnvelope = true;

            //if (hotelExtension != null)
            //{
            //    isGuidePrice = hotelExtension.Extension3 ?? 0;
            //    isReduce = hotelExtension.Extension ?? false;         //价格控制总阀
            //    deductPoint = hotelExtension.Extension4 == 1;     //下降一个点
            //    isAddRedEnvelope = hotelExtension.Extension5 == 1;
            //}

            if (isGuidePrice == 1)
            {
                return guidePrice;
            }
            #region
            /*
            #region 接口获取matt那边的卖价，获取不到则返回指导价 [190404]
            try
            {
                //单位分
                var mattSalePrice = Convert.ToInt32(WebHttpRequest.Get(string.Format("http://119.23.44.20:9000/AliTrip/GetSalePrice?Channel=5&CurDate={0}&GuidePrice={1}&SubRatio={2}&IsAgent={3}&IsVirtual={4}&IsMedalHotel=false&IsHzHotel={5}", date.ToString("yyyy-MM-dd"), guidePrice, subRate, isCommission, isVirtual, !isAddRedEnvelope)));
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
            #region 190319
            //如果美团佣金率>6.5%的，可以再低1个点
            //100-220减1，
            //220到320 减2
            //320到450 减3
            //450到600 减4
            //salePrice>600的 减5
            //salePrice>800的 减6
            if (subRate > 0.055m)
            {
                salePrice = Math.Round(guidePrice * 0.975m, 2);
            }
            else
            {
                salePrice = Math.Round(guidePrice * 0.98m, 2);
            }
            if (salePrice >= 100 && salePrice <= 220)
            {
                salePrice += -1;
            }
            else if (salePrice > 220 && salePrice <= 320)
            {
                salePrice += -2;
            }
            else if (salePrice > 320 && salePrice <= 450)
            {
                salePrice += -3;
            }
            else if (salePrice > 450 && salePrice <= 600)
            {
                salePrice += -4;
            }
            else if (salePrice > 600 && salePrice <= 800)
            {
                salePrice += -5;
            }
            else if (salePrice > 800)
            {
                salePrice += -6;
            }
            //美团代理产品在先有基础上多加一个点
            if (isCommission)
            {
                salePrice = Math.Ceiling(salePrice * 1.01m);
            }
            if (isVirtual) //虚拟价格这个基础上统一减1，然后模拟出来的不设置免费取消，取消都要收55%，不能取消的时间点到当日22点，取消收69%
            {
                salePrice += -1;
            }

            //平时的加价提高0.3%
            salePrice = salePrice * 1.003m;
            //距离目前超过30天以上的日期的报价，你多加0.5个点
            if ((date - DateTime.Now.Date).TotalDays > 30)
            {
                salePrice = salePrice * 1.005m;
            }
            //7号凌晨前，美团产品90-300的可以再调低1元，300以上调低2元
            if (DateTime.Now < new DateTime(2019, 4, 7))
            {
                if (salePrice >= 90 && salePrice <= 300)
                {
                    salePrice += -1;
                }
                else if (salePrice > 300)
                {
                    salePrice += -2;
                }
            }

            salePrice = (int)salePrice + 0.49m;
            #endregion
            salePrice += 20;//加20红包再减20
            */
            #endregion
            //代理加一个点
            //if (isCommission)
            //{
            //    salePrice = salePrice * 1.01M;
            //}
            //salePrice = Math.Round(salePrice * 0.995m) + 0.49m;

            //if (isCommission)
            //{
            //    salePrice = salePrice * 1.01M;//代理加一个点
            //}
            if (salePrice >= 70 && salePrice <= 230)
            {
                salePrice = salePrice * 0.998m;
            }

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
                    //当降价了，直接输出之前推送的价格
                    //if (Convert.ToDecimal(inventory_price.price) / 100 >= salePrice * 0.97m || (salePrice - Convert.ToDecimal(inventory_price.price) / 100) <= 40)
                    if (Convert.ToDecimal(inventory_price.price) / 100 >= salePrice * 0.97m || (salePrice - Convert.ToDecimal(inventory_price.price) / 100) <= 10)
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
                    //else if ((inventory_price.price / 100) * 1.01m > salePrice)
                    //{
                    //    return inventory_price.price / 100;
                    //}
                }
            }
            
            return salePrice;
        }


        /// <summary>
        /// 获取酒店原始信息
        /// </summary>
        /// <param name="checkDto"></param>
        /// <returns></returns>
        public TmallHotelPriceInfo GetHotelPriceInfo(BookingCheckInputDto checkDto)
        {
            long hotelId = Convert.ToInt64(checkDto.HotelId);
            int rpid = Convert.ToInt32(checkDto.RatePlanId);
            TmallHotelPriceInfo priceInfo = new TmallHotelPriceInfo();
            var request = new HotelGoodsRequest
            {
                CheckinDate = checkDto.CheckIn.ToString("yyyy-MM-dd"),
                CheckoutDate = checkDto.CheckOut.ToString("yyyy-MM-dd"),
                GoodsType = 1,
                HotelIds = new List<long> { hotelId
                }
            };
            //当失败时重新试单
            var response = new HotelGoodsResponse();
            try
            {
                response = meiTuanApiClient.Excute(request);
            }
            catch (Exception exMt)
            {
                response = meiTuanApiClient.Excute(request);
            }
            if (response != null && response.Result != null && response.Result.hotelGoods != null && response.Result.hotelGoods.Count > 0)
            {
                var rplist = response.Result.hotelGoods[0].goods;
                var currentRp = rplist.Where(u => u.hotelId == hotelId && u.goodsId == rpid).FirstOrDefault();
                if (currentRp != null)
                {
                    priceInfo.RoomName = currentRp.roomInfoList[0].roomName;
                    priceInfo.RatePlanName = currentRp.goodsName;
                    int invoiceMode = currentRp.invoiceInfo.invoiceMode;
                    if (invoiceMode == 1)
                    {
                        priceInfo.RatePlanName = string.Format("{0}[酒店开具发票]", priceInfo.RatePlanName);
                    }
                    else if (invoiceMode == 2)
                    {
                        priceInfo.RatePlanName = string.Format("{0}[美团开具发票]", priceInfo.RatePlanName);
                    }
                    else if (invoiceMode == 3)
                    {
                        priceInfo.RatePlanName = string.Format("{0}[第三方开具发票]", priceInfo.RatePlanName);
                    }

                    if (currentRp.thirdParty == 1)
                    {
                        priceInfo.RatePlanName = string.Format("{0}[代理]", priceInfo.RatePlanName);
                    }

                    if (currentRp.goodsType == 2)
                    {
                        priceInfo.RatePlanName = string.Format("{0}[钟点房]", priceInfo.RatePlanName);
                    }

                    //priceInfo.RatePlanName = string.Format("{0} [{1}早]", priceInfo.RatePlanName, );
                    string priceStr = string.Empty;
                    decimal sTotalPrice = 0;
                    decimal sBasePrice = 0;

                    foreach (var item in currentRp.priceModels)
                    {
                        var subPricedic = item.subPrice;
                        decimal basePrice = item.salePrice - item.subPrice;
                        sBasePrice += basePrice;
                        sTotalPrice += subPricedic;
                        priceStr += "price" + basePrice / 100 + "|" + item.salePrice / 100 + "|money" + basePrice / 100 + "|0|";
                    }
                    priceInfo.DatePrice = Convert.ToDecimal(sBasePrice / 100);
                    priceInfo.PriceStr = priceStr;
                    priceInfo.PaymentType = 1;
                    priceInfo.STotalPrice = Convert.ToDecimal(sTotalPrice / 100);


                    var hotel = SqlSugarContext.ChannelInstance.Queryable<MeiTuanHotel>().Where(u => u.HotelId == hotelId).First();
                    if (hotel != null)
                    {
                        priceInfo.HotelName = hotel.HotelName;
                    }
                    else
                    {
                        try
                        {
                            string sql = string.Format("select top 1 hotelId as HotelId,pointName as HotelName from Mt_hotels where hotelId = {0}", hotelId);
                            var getHotelInfo = SqlSugarContext.HotelProductInstance.SqlQueryable<MeiTuanHotel>(sql).First();
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
                    }
                }
            }


            return priceInfo;
        }


        /// <summary>
        /// 获取价格计划发票
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public RatePlanInvoice GetRatePlanInvoice(string hotelId, string ratePlanId, DateTime checkIn, DateTime checkOut)
        {
            var hid = Convert.ToInt32(hotelId);
            var rpid = Convert.ToInt32(ratePlanId);
            RatePlanInvoice ratePlanInfo = null;
            var request = new HotelGoodsRequest
            {
                HotelIds = new List<long> { hid },
                CheckinDate = checkIn.ToString("yyyy-MM-dd"),
                CheckoutDate = checkOut.ToString("yyyy-MM-dd"),
                GoodsType = 1
            };
            var response = meiTuanApiClient.Excute(request);
            if (response.Result != null && response.Result.hotelGoods != null && response.Result.hotelGoods.Count > 0)
            {
                var hotelDetail = response.Result.hotelGoods.FirstOrDefault();
                var goods = hotelDetail.goods.Where(u => u.hotelId == hid && u.goodsId == rpid).FirstOrDefault();
                if (goods != null)
                {
                    ratePlanInfo = new RatePlanInvoice();
                    ratePlanInfo.HotelId = hid;
                    ratePlanInfo.RoomId = goods.roomInfoList[0].roomId;
                    ratePlanInfo.InvoiceMode = (short)goods.invoiceInfo.invoiceMode;
                    ratePlanInfo.IsCommission = goods.thirdParty == 1;//0为非代理
                }
            }
            else
            {
                return meituanRepository.GetRatePlanInvoice(hotelId, ratePlanId);
            }

            return ratePlanInfo;
        }

        /// <summary>
        /// 获取试单前的总金额
        /// </summary>
        /// <param name="checkDto"></param>
        /// <returns></returns>
        public decimal getTotaPrice(BookingCheckInputDto checkDto)
        {
            decimal salePrice = 0;
            try
            {
                long hotelId = Convert.ToInt64(checkDto.HotelId);
                int rpid = Convert.ToInt32(checkDto.RatePlanId);
                var request = new HotelGoodsRequest
                {
                    CheckinDate = checkDto.CheckIn.ToString("yyyy-MM-dd"),
                    CheckoutDate = checkDto.CheckOut.ToString("yyyy-MM-dd"),
                    GoodsType = 1,
                    HotelIds = new List<long> { hotelId
                }
                };
                var response = new HotelGoodsResponse();
                try
                {
                    response = meiTuanApiClient.Excute(request);
                }
                catch (Exception exstr)
                {
                    response = meiTuanApiClient.Excute(request);
                }
                if (response != null && response.Result != null && response.Result.hotelGoods != null && response.Result.hotelGoods.Count > 0)
                {
                    var rplist = response.Result.hotelGoods[0].goods;
                    var currentRp = rplist.Where(u => u.hotelId == hotelId && u.goodsId == rpid).FirstOrDefault();
                    if (currentRp != null)
                    {


                        //priceInfo.RatePlanName = string.Format("{0} [{1}早]", priceInfo.RatePlanName, );
                        string priceStr = string.Empty;
                        decimal sTotalPrice = 0;
                        decimal sBasePrice = 0;


                        foreach (var item in currentRp.priceModels)
                        {
                            var subPricedic = item.subPrice;
                            decimal basePrice = item.salePrice - item.subPrice;
                            sBasePrice += basePrice;
                            sTotalPrice += subPricedic;
                            salePrice += item.salePrice;
                            priceStr += "price" + basePrice / 100 + "|" + item.salePrice / 100 + "|money" + basePrice / 100 + "|0|";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
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
