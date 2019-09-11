using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Entities.Domestic
{
    public class DomesticRatePlan
    {
        public string Id { get; set; }

        public string Hid { get; set; }

        public string Rid { get; set; }

        public string RoomTypeId { get; set; }

        public string Name { get; set; }

        public short Status { get; set; }

        public short BreakCount { get; set; }

        /// <summary>
        /// 最小入住天数（1-90）
        /// </summary>
        public short MinDays { get; set; }

        /// <summary>
        /// 最大入住天数（1-90）
        /// </summary>
        public short MaxDays { get; set; }

        /// <summary>
        /// 最大提前预定小时数
        /// </summary>
        public short MaxAdvHours { get; set; }

        /// <summary>
        /// 最小提前预定小时数
        /// </summary>
        public short MinAdvHours { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CurrencyCode { get; set; }

        public short? InvoiceMode { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        /// <summary>
        /// 是否即时确认
        /// </summary>
        public bool? InstantConfirmation { get; set; }

        /// <summary>
        /// 最小入住间数
        /// </summary>
        public short? MinAmount { get; set; }

        /// <summary>
        /// 早餐日历
        /// </summary>
        public string BreakfastCal { get; set; }

        public int CancelDay { get; set; }
        public int CancelTime { get; set; }

        public string CancelPolicy { get; set; }

        public decimal? subRatio { get; set; }

        /// <summary>
        /// 是否代理
        /// </summary>
        public bool? IsCommissionable { get; set; }


        public List<decimal> NightAmount { get; set; }


    }
}
