using DaDuShi.Common;
using DaDuShi.Request;
using DaDuShi.Response;
using DaDuShi.DaDSService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DaDuShi.Entities
{
    public class OrderDataOperate : BaseOperate
    {
        #region 外部调用方法

        /// <summary>
        /// 获取价格信息
        /// </summary>
        /// <returns></returns>
        public WebServiceResponse<HotelInfoJsonEt> GetHotelRatePlan(GetRatePlanJsonEt RequestEt, bool isRealTime = false)
        {

            WebServiceResponse<HotelInfoJsonEt> jsonEt = null;
            try
            {
                #region Web服务对象 请求数据
                // Web服务对象 请求数据
                var responseEt = _Request.RateChange(RequestEt.HotelCode, (RequestEt.AdultCount + RequestEt.ChildCount).ToString(), RequestEt.CheckIn, RequestEt.CheckOut);

                // 处理 Response对象
                jsonEt = HandleResponse<HotelInfoJsonEt>(responseEt.Error);

                if (jsonEt.Successful)
                {
                    // 成功
                    jsonEt.ResponseEt = null;
                    if (responseEt != null && responseEt.RateChange != null)
                    {
                        // 封装 酒店信息
                        jsonEt.ResponseEt = new HotelInfoJsonEt()
                        {
                            HotelCode = RequestEt.HotelCode,
                            Inform = responseEt.RateChange.Inform,
                            LastOrderTime = responseEt.RateChange.LastOrderTime,
                            RatePlanList = new List<RatePlanJsonEt>()
                        };

                        var roomRates = responseEt.RateChange.RoomRates;
                        if (roomRates != null && roomRates.Length > 0)
                        {
                            // 遍历价格计划信息
                            foreach (var _RoomRate in roomRates)
                            {
                                try
                                {
                                    var rpJsonEt = FillRatePlanJsonEt(_RoomRate);
                                    if (rpJsonEt != null)
                                        jsonEt.ResponseEt.RatePlanList.Add(rpJsonEt);
                                }
                                catch (Exception ex)
                                {
                                    string errMsg = string.Format("封装价格计划 异常：{0}", ex.Message);
                                    // 保存操作日志

                                }
                            }

                            if (jsonEt.ResponseEt.RatePlanList.Count == 0)
                            {
                                jsonEt.ResponseEt = null;
                            }
                        }
                        else
                        {
                            jsonEt.ResponseEt = null;
                        }
                    }
                    else
                    {
                        jsonEt.ResponseEt = null;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            { }

            return jsonEt;
        }

        /// <summary>
        /// 价格确认
        /// </summary>
        public WebServiceResponse<HotelInfoJsonEt> Availability(RatePlanConfrimJsonEt RequestEt)
        {
            // Web服务对象 请求数据
            var responseEt = _Request.Availability(RequestEt.HotelCode, RequestEt.RatePlanCode, RequestEt.RoomTypeCode, RequestEt.NumberOfUnits, RequestEt.CheckIn, RequestEt.CheckOut, RequestEt.AdultCount, RequestEt.ChildCount);
            // 处理 Response对象
            var respon = new WebServiceResponse<HotelInfoJsonEt>();
            var jsonEt = HandleResponse<HotelInfoJsonEt>(responseEt.Error);

            if (jsonEt.Successful)
            {
                // 成功
                if (responseEt != null && responseEt.Availability != null)
                {
                    respon.ResponseEt = new HotelInfoJsonEt()
                    {
                        HotelCode = RequestEt.HotelCode,
                        RatePlanList = new List<RatePlanJsonEt>()
                    };
                    string RoomTypeName = "";
                    string RatePlanName = "";
                    if (responseEt.Availability.RoomTypes != null && responseEt.Availability.RoomTypes.Length > 0)
                    {
                        RoomTypeName = responseEt.Availability.RoomTypes[0].RoomTypeName;
                    }
                    if (responseEt.Availability.RatePlans != null && responseEt.Availability.RatePlans.Length > 0)
                    {
                        RatePlanName = responseEt.Availability.RatePlans[0].RatePlanName;
                    }

                    var roomRates = responseEt.Availability.RoomRates;
                    if (roomRates != null && roomRates.Length > 0)
                    {
                        // 遍历价格计划信息
                        foreach (var _RoomRate in roomRates)
                        {
                            try
                            {
                                var rpJsonEt = FillRatePlanJsonEt(_RoomRate);
                                if (rpJsonEt != null)
                                {
                                    rpJsonEt.RoomTypeName = RoomTypeName;
                                    rpJsonEt.RatePlanName = RatePlanName;
                                    respon.ResponseEt.RatePlanList.Add(rpJsonEt);
                                }
                            }
                            catch (Exception ex)
                            {
                                string errMsg = string.Format("封装价格计划 异常：{0}", ex.Message);
                                // 保存操作日志
                                //BLL.Log.Initialize().SaveErrLog(OperationCode.获取价格确认数据, errMsg, JsonUtil.GetJsonByObj(_RoomRate), JsonUtil.GetJsonByObj(RequestEt));
                            }
                        }

                        if (respon.ResponseEt.RatePlanList.Count == 0)
                        {
                            respon.ResponseEt = null;
                        }
                    }
                    else
                    {
                        respon.ResponseEt = null;
                    }
                }
                else
                {
                    respon.ResponseEt = null;
                }
            }

            return respon;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="RequestEt"></param>
        public WebServiceResponse<OrderInfoJsonEt> Reservation(CreateOrderJsonEt RequestEt)
        {
            #region 封装 订单信息对象
            Reservation reservation = new Reservation()
            {
                HotelCode = RequestEt.HotelCode,
                RatePlanCode = RequestEt.RatePlanCode,
                RoomTypeCode = RequestEt.RoomTypeCode,
                NumberOfUnits = RequestEt.NumberOfUnits,
                DistributorReservationId = RequestEt.DistributorReservationId,
                ReservationStatus = ReservationStatus.Confirmed,
                Total = new Total()
                {
                    TotalAmount = RequestEt.TotalAmount
                },
                StayDateRange = new StayDateRange()
                {
                    CheckIn = RequestEt.CheckIn,
                    CheckOut = RequestEt.CheckOut
                },
                ContactPerson = new Customer()
                {
                    GivenName = RequestEt.ContactPerson.GivenName,
                    Surname = RequestEt.ContactPerson.Surname,
                    Telephone = RequestEt.ContactPerson.Telephone,
                    Email = RequestEt.ContactPerson.Email,
                    Address = RequestEt.ContactPerson.Address,
                },
                GuestCount = new DaDSService.GuestCount()
                {
                    AdultCount = RequestEt.GuestCount.AdultCount,
                    ChildCount = RequestEt.GuestCount.ChildCount
                },
                Comments = new string[1] { RequestEt.Comment },
            };

            List<ReservationGuest> rGuestList = new List<ReservationGuest>();
            foreach (var _Guest in RequestEt.GuestList)
            {
                ReservationGuest rGuest = new ReservationGuest()
                {
                    GivenName = _Guest.GivenName,
                    Surname = _Guest.Surname,
                    Telephone = _Guest.Telephone,
                    Email = _Guest.Email
                };
                rGuestList.Add(rGuest);
            }

            reservation.Guests = rGuestList.ToArray();
            #endregion

            // Web服务对象 提交数据
            var responseEt = _Request.Reservation(reservation);
            // 处理 Response对象
            var jsonEt = HandleResponse<OrderInfoJsonEt>(responseEt.Error);
            if (jsonEt.Successful)
            {
                jsonEt.ResponseEt = new OrderInfoJsonEt()
                {
                    ReservationStatus = responseEt.ReservationStatus.ToString(),
                    TotalAmount = responseEt.Total.TotalAmount,
                    ProductReservationId = responseEt.ReservationId.ProductReservationId,
                    ErpReservationId = responseEt.ReservationId.ErpReservationId,
                    DistributorReservationId = RequestEt.DistributorReservationId,
                    CancelTime = responseEt.CancelTime
                };
            }

            return jsonEt;
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="RequestEt"></param>
        public WebServiceResponse<CancelOrderStatusJsonEt> Cancel(OrderRequestJsonEt RequestEt)
        {
            // Web服务对象 提交数据
            var responseEt = _Request.Cancel(RequestEt.ProductReservationId, "", "");
            // 处理 Response对象
            var jsonEt = HandleResponse<CancelOrderStatusJsonEt>(responseEt.Error);
            if (jsonEt.Successful)
            {
                jsonEt.ResponseEt = new CancelOrderStatusJsonEt()
                {
                    ProductReservationId = RequestEt.ProductReservationId,
                    DistributorReservationId = RequestEt.DistributorReservationId,
                    Status = responseEt.Status.ToString()
                };
            }

            return jsonEt;
        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="RequestEt"></param>
        /// <returns></returns>
        public WebServiceResponse<OrderQueryJsonEt> OrderQuery(OrderRequestJsonEt RequestEt)
        {
            // Web服务对象 提交数据
            var responseEt = _Request.OrderQuery(RequestEt.DistributorReservationId);
            // 处理 Response对象
            var jsonEt = HandleResponse<OrderQueryJsonEt>(responseEt.Error);
            if (jsonEt.Successful)
            {
                jsonEt.ResponseEt = null;
                if (responseEt != null && responseEt.OrderQueryInfo != null)
                {
                    var OrderInfos = responseEt.OrderQueryInfo.OrderInfos;
                    if (OrderInfos != null && OrderInfos.Length > 0)
                    {
                        jsonEt.ResponseEt = new OrderQueryJsonEt()
                        {
                            DistributorReservationId = RequestEt.DistributorReservationId,
                            ProductReservationId = OrderInfos[0].ProductReservationId,
                            ConfirmNo = OrderInfos[0].ConfirmNo,
                            ERPReservationId = OrderInfos[0].ERPReservationId,
                            ReservationStatus = OrderInfos[0].ReservationStatus,
                            TotalAmount = OrderInfos[0].TotalAmount
                        };
                    }
                }
            }

            return jsonEt;
        }
        #endregion

        #region 内部调用方法
        /// <summary>
        /// 封装 RatePlanJsonEt
        /// </summary>
        /// <param name="RoomRate"></param>
        /// <returns></returns>
        private RatePlanJsonEt FillRatePlanJsonEt(RoomRate RoomRate)
        {
            bool isAddRatePlan = true;
            RatePlanJsonEt rpJsonEt = new RatePlanJsonEt()
            {
                RoomTypeCode = RoomRate.RoomTypeCode,
                RoomTypeName = RoomRate.RoomTypeName,
                RatePlanCode = RoomRate.RatePlanCode,
                RatePlanName = RoomRate.RatePlanName,
                CurrencyCode = RoomRate.CurrencyCode,
                PaymentType = RoomRate.PaymentType,
                LastDays = RoomRate.LastDays,
                AdvanceDays = RoomRate.AdvanceDays,
                AdvanceHours = RoomRate.AdvanceHours,
                PaymentPolicies = RoomRate.PaymentPolicies,
                StopCancelDate = RoomRate.StopCancelDate,
                RateCancelPolicy = RoomRate.RateCancelPolicy,
                RateNotUpdateable = RoomRate.RateNotUpdateable,
                RTBreakfast = RoomRate.RTBreakfast,
                BedTypeName = RoomRate.BedTypeName,
                RTStopCancelTime = RoomRate.RTStopCancelTime,
                RoomTypeUseBeforeTime = RoomRate.RoomTypeUseBeforeTime,
                RateList = new List<RateJsonEt>()
            };

            #region 封装 价格信息
            if (RoomRate.Rates != null && RoomRate.Rates.Length > 0)
            {
                foreach (var _Rate in RoomRate.Rates)
                {
                    if (_Rate.Ability != -1)
                    {
                        RateJsonEt rJsonEt = new RateJsonEt()
                        {
                            EffectiveDate = _Rate.EffectiveDate,
                            ExpireDate = _Rate.ExpireDate,
                            AmountBeforeTax = _Rate.AmountBeforeTax,
                            AmountAfterTax = _Rate.AmountAfterTax,
                            Breakfast = _Rate.Breakfast,
                            Adult = _Rate.Adult,
                            Child = _Rate.Child,
                            Ability = _Rate.Ability,
                            LosSpecified = _Rate.LosSpecified
                        };

                        // 价格 加入到该价格计划下的RateList
                        rpJsonEt.RateList.Add(rJsonEt);
                    }
                    else
                    {
                        // 价格计划中，如果其中一个Rate没有价格，则该价格计划不需要显示
                        isAddRatePlan = false;
                        break;
                    }
                }
            }
            else
            {
                // 价格计划 没有价格信息，则该价格计划不需要显示
                isAddRatePlan = false;
            }
            #endregion

            if (isAddRatePlan)
                return rpJsonEt;

            return null;
        }
        #endregion
    }
}
