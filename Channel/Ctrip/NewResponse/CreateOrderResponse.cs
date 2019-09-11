using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtripApi.NewResponse
{
    public class CreateOrderResponse : BaseResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public List<WarningsItem> Warnings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HotelReservations HotelReservations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ErrorsItem> Errors { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TimeStamp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PrimaryLangID { get; set; }

    }


    public class ErrorsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShortText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }



    public class WarningsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 该房型不支持送券
        /// </summary>
        public string ShortText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 110117000105-该房型不支持送券
        /// </summary>
        public string Value { get; set; }
    }

    public class RatePlan
    {
        /// <summary>
        /// 
        /// </summary>
        public string RatePlanCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RoomID { get; set; }
    }

    public class RatePlans
    {
        /// <summary>
        /// 
        /// </summary>
        public RatePlan RatePlan { get; set; }
    }

    public class RoomStay
    {
        /// <summary>
        /// 
        /// </summary>
        public RatePlans RatePlans { get; set; }
    }

    public class RoomStays
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomStay RoomStay { get; set; }
    }

    public class CancelPenaltiesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Start { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string End { get; set; }
    }

    public class TPA_ExtensionsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int ExclusiveAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int InclusiveAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrencyCode { get; set; }
    }

    public class Total
    {
        /// <summary>
        /// 
        /// </summary>
        public List<TPA_ExtensionsItem> TPA_Extensions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ExclusiveAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int InclusiveAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrencyCode { get; set; }
    }

    public class HotelReservationIDsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ResID_Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ResID_Value { get; set; }
    }

    public class DepositPaymentsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string GuaranteeCode { get; set; }
    }

    public class ResGlobalInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DepositPaymentsItem> DepositPayments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CancelPenaltiesItem> CancelPenalties { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Total Total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<HotelReservationIDsItem> HotelReservationIDs { get; set; }
    }

    public class TPA_Extensions
    {
    }

    public class HotelReservation
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomStays RoomStays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ResGlobalInfo ResGlobalInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TPA_Extensions TPA_Extensions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ResStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsInstantConfirm { get; set; }
    }

    public class HotelReservations
    {
        /// <summary>
        /// 
        /// </summary>
        public HotelReservation HotelReservation { get; set; }
    }

}