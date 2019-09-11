using CtripApi.NewResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtripApi.NewRequest
{
    public class CreateOrderRequest : ICtripRequest<CreateOrderResponse>
    {

        public string ICODE => "2b0c7e0eebe2467a97be3d284313a129";

        /// <summary>
        /// 
        /// </summary>
        public List<UniqueIDItem> UniqueID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HotelReservations HotelReservations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PrimaryLangID { get; set; }
    }




    public class UniqueIDItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }
    }

    public class RoomType
    {
        /// <summary>
        /// 
        /// </summary>
        public int NumberOfUnits { get; set; }
    }

    public class RoomTypes
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomType RoomType { get; set; }
    }

    public class RatePlan
    {
        /// <summary>
        /// 
        /// </summary>
        public int PrepaidIndicator { get; set; }
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

    public class BasicPropertyInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string HotelCode { get; set; }
    }

    public class RoomStay
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomTypes RoomTypes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RatePlans RatePlans { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BasicPropertyInfo BasicPropertyInfo { get; set; }
    }

    public class RoomStays
    {
        /// <summary>
        /// 
        /// </summary>
        public RoomStay RoomStay { get; set; }
    }

    public class PersonNameItem
    {
        /// <summary>
        /// 刘杰华
        /// </summary>
        public string Name { get; set; }
    }

    public class PersonName
    {
        /// <summary>
        /// 刘杰华
        /// </summary>
        public string Name { get; set; }
    }

    public class Telephone
    {
        /// <summary>
        /// 
        /// </summary>
        public string PhoneTechType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
    }

    public class ContactPerson
    {
        /// <summary>
        /// 
        /// </summary>
        public PersonName PersonName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Telephone Telephone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ContactType { get; set; }
    }

    public class Customer
    {
        /// <summary>
        /// 
        /// </summary>
        public List<PersonNameItem> PersonName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ContactPerson ContactPerson { get; set; }
    }

    public class Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public Customer Customer { get; set; }
    }

    public class ProfileInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Profile Profile { get; set; }
    }

    public class Profiles
    {
        /// <summary>
        /// 
        /// </summary>
        public ProfileInfo ProfileInfo { get; set; }
    }

    public class TPA_Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        public string LateArrivalTime { get; set; }
    }

    public class ResGuest
    {
        /// <summary>
        /// 
        /// </summary>
        public Profiles Profiles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TPA_Extensions TPA_Extensions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ArrivalTime { get; set; }
    }

    public class ResGuests
    {
        /// <summary>
        /// 
        /// </summary>
        public ResGuest ResGuest { get; set; }
    }

    public class GuestCount
    {
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
    }

    public class GuestCounts
    {
        /// <summary>
        /// 
        /// </summary>
        public GuestCount GuestCount { get; set; }
    }

    public class TimeSpanInfo
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

    public class Total
    {
        /// <summary>
        /// 
        /// </summary>
        public decimal ExclusiveAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InclusiveAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrencyCode { get; set; }
    }

    public class TotalCost
    {
        /// <summary>
        /// 
        /// </summary>
        public int ExclusiveAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal InclusiveAmount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrencyCode { get; set; }
    }

    public class TPA_Extensions1
    {
        /// <summary>
        /// 
        /// </summary>
        public TotalCost TotalCost { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Invoice { get; set; }
    }

    public class ResGlobalInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public GuestCounts GuestCounts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpanInfo TimeSpan { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> DepositPayments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Total Total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TPA_Extensions1 TPA_Extensions { get; set; }
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
        public ResGuests ResGuests { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ResGlobalInfo ResGlobalInfo { get; set; }
    }

    public class HotelReservations
    {
        /// <summary>
        /// 
        /// </summary>
        public HotelReservation HotelReservation { get; set; }
    }


}