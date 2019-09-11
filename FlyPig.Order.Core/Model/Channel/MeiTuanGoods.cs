using System;
using System.Linq;
using System.Text;

namespace FlyPig.Order.Core.Model
{
    ///<summary>
    ///
    ///</summary>
    public partial class MeiTuanGoods
    {
        public MeiTuanGoods()
        {


        }

        public override bool Equals(object obj)
        {
            var rp = obj as MeiTuanGoods;

            if (GoodsName != rp.GoodsName || GoodsStatus != rp.GoodsStatus || BreakfastCount != rp.BreakfastCount || CancelHours != rp.CancelHours
                || InvoiceInfo != rp.InvoiceInfo || CancelDays != rp.CancelDays || SerialCheckinMin != rp.SerialCheckinMin || SerialCheckinMax != rp.SerialCheckinMax ||
                 RoomCountMin != rp.RoomCountMin || RoomCountMax != rp.RoomCountMax || EarliestBookingDays != rp.EarliestBookingDays || EarliestBookingHours != rp.EarliestBookingHours ||
               latestBookingDays != rp.latestBookingDays || latestBookingHours != rp.latestBookingHours ||
               IsDaybreakBooking != rp.IsDaybreakBooking || ThirdParty != rp.ThirdParty
                )
            {
                return false;
            }
            else
            {
                return true;
            }


            //   SerialCheckinMin SerialCheckinMax     RoomCountMin   RoomCountMax    EarliestBookingDays    EarliestBookingHours
            // latestBookingDays   latestBookingHours      IsDaybreakBooking
        }




        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int HotelId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int RoomId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public long GoodsId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string GoodsName { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? NeedRealTel { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? GoodsStatus { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? GoodsType { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? ConfirmType { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? InvRemain { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? BreakfastCount { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? CancelDays { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? CancelHours { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? InvoiceInfo { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? SerialCheckinMin { get; set; }

        public int? ThirdParty { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? SerialCheckinMax { get; set; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? RoomCountMin { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? RoomCountMax { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? EarliestBookingDays { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string EarliestBookingHours { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? latestBookingDays { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string latestBookingHours { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? IsDaybreakBooking { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? ModifyTime { get; set; }

    }
}
