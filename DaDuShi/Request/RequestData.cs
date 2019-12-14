using DaDuShi.DaDSService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Request
{
    public class RequestData
    {
        #region 初始化
        private string _RequestName { get; set; }

        //public DaDuShi.Common.DaDuShiConfig Config { get; set; }
        /// <summary>
        /// Web服务引用对象
        /// </summary>
        private StandardClient _Client;
        public RequestData()
        {
            try
            {
                _RequestName = "DaDSRequest";
                _Client = new StandardClient();
            }
            catch(Exception ex)
            {
            }
        }
        #endregion

        public class Config
        {
            public static string ClientID = "C668434";
            public static string LicenseKey = "Toptown#C668434";
            public static string Token = "";
        }


        #region 外部调用方法
        /// <summary>
        /// 根据起止日期查询可售酒店信息
        /// </summary>
        /// <param name="strartDate"></param>
        /// <param name="endDate"></param>
        public SalableHotelResponse SalableHotel(string StrartDate, string EndDate)
        {
            SalableHotelRequest request = new SalableHotelRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                SalableHotelCriteria = new SalableHotelCriteria()
                {
                    DateRange = new DateRange()
                    {
                        startDate = StrartDate,
                        endDate = EndDate
                    }
                }
            };

            return _Client.SalableHotel(request);
        }

        /// <summary>
        /// 提供酒店基础信息的查询
        /// </summary>
        /// <param name="HotelCode"></param>
        /// <returns></returns>
        public HotelBasicInfoResponse HotelBasicInfo(List<string> HotelCode)
        {
            HotelBasicInfoRequest request = new HotelBasicInfoRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                HotelBasicInfoCriteria = new HotelBasicInfoCriteria()
                {
                    HotelQueryType = HotelQueryType.HotelCode,
                    HotelQuery = HotelCode.ToArray()
                }
            };

            return _Client.HotelBasicInfo(request);
        }

        /// <summary>
        /// 获取此酒店在此指定时间范围内的价格
        /// </summary>
        /// <param name="HotelCode"></param>
        /// <param name="Los"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public RateChangeResponse RateChange(string HotelCode, string Los, string StartDate, string EndDate)
        {
            RateChangeRequest request = new RateChangeRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                ChangeRateCriteria = new ChangeRateCriteria()
                {
                    HotelCode = HotelCode,
                    Los = Los,
                    DateRange = new DateRange()
                    {
                        startDate = StartDate,
                        endDate = EndDate
                    }
                }
            };

            return _Client.RateChange(request);
        }

        /// <summary>
        /// 查询指定时间段信息（房价）有变化的酒店，以便及时获得大都市系统内酒店房态和房价实时的变化情况。
        /// </summary>
        /// <param name="HotelCode"></param>
        /// <param name="Timestamp"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public HotelChangeResponse HotelChange(string HotelCode, string Timestamp, string StartDate, string EndDate)
        {
            HotelChangeRequest request = new HotelChangeRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                HotelChangeCriteria = new HotelChangeCriteria()
                {
                    HotelCode = HotelCode,
                    Timestamp = Timestamp,
                    DateRange = new DateRange()
                    {
                        startDate = StartDate,
                        endDate = EndDate
                    }
                }
            };

            return _Client.HotelChange(request);
        }

        /// <summary>
        /// 查询指定时间段信息有变化的酒店，以便及时获得大都市系统内酒店房态和房价实时的变化情况
        /// </summary>
        /// <param name="Timestamp"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public HotelChangeDetailResponse HotelChangeDetail(string Timestamp, string StartDate, string EndDate)
        {
            HotelChangeDetailRequest request = new HotelChangeDetailRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                HotelChangeDetailCriteria = new HotelChangeDetailCriteria()
                {
                    Timestamp = Timestamp,
                    DateRange = new DateRange()
                    {
                        startDate = StartDate,
                        endDate = EndDate
                    }
                }
            };

            return _Client.HotelChangeDetail(request);

        }

        /// <summary>
        /// 用于可订检查。客户预订前调用，检查某酒店哪些房型是否可订的,返回房价
        /// </summary>
        /// <returns></returns>
        public AvailabilityResponse Availability(string HotelCode, string RatePlanCode, string RoomTypeCode, int NumberOfUnits, string CheckIn, string CheckOut, int AdultCount, int ChildCount)
        {
            AvailabilityRequest request = new AvailabilityRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                AvailabilityCriteria = new AvailabilityCriteria()
                {
                    HotelCode = HotelCode,
                    RatePlanCode = RatePlanCode,
                    RoomTypeCode = RoomTypeCode,
                    NumberOfUnits = NumberOfUnits,
                    GuestCount = new GuestCount()
                    {
                        AdultCount = AdultCount,
                        ChildCount = ChildCount
                    },
                    StayDateRange = new StayDateRange()
                    {
                        CheckIn = CheckIn,
                        CheckOut = CheckOut
                    }
                }
            };

            return _Client.Availability(request);
        }

        /// <summary>
        /// 新增订单接口
        /// </summary>
        /// <returns></returns>
        public ReservationResponse Reservation(Reservation Reservation)
        {
            ReservationRequest request = new ReservationRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                Reservation = Reservation
            };

            return _Client.Reservation(request);
        }

        /// <summary>
        /// 取消订单接口
        /// </summary>
        /// <param name="ProductReservationId"></param>
        /// <param name="ErpReservationId"></param>
        /// <param name="HotelReservationId"></param>
        /// <returns></returns>
        public CancelResponse Cancel(string ProductReservationId, string ErpReservationId, string HotelReservationId)
        {
            CancelRequest request = new CancelRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                ReservationId = new ReservationId()
                {
                    ProductReservationId = ProductReservationId,
                    ErpReservationId = ErpReservationId,
                    HotelReservationId = HotelReservationId
                }
            };

            return _Client.Cancel(request);
        }

        /// <summary>
        /// 根据订单ID查询订单信息
        /// </summary>
        /// <returns></returns>
        public OrderQueryResponse OrderQuery(string ReservationId)
        {
            OrderQueryRequest request = new OrderQueryRequest()
            {
                UserName = Config.ClientID,
                Password = Config.LicenseKey,
                Token = Config.Token,
                ReservationId = ReservationId
            };

            return _Client.OrderQuery(request);
        }
        #endregion
    }
}
