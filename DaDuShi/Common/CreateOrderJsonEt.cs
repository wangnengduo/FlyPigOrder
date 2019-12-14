using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Common
{
    public class CreateOrderJsonEt
    {
        /// <summary>
        /// 酒店代码
        /// </summary>
        public string HotelCode { get; set; }
        /// <summary>
        /// 价格计划代码
        /// </summary>
        public string RatePlanCode { get; set; }
        /// <summary>
        /// 房型代码
        /// </summary>
        public string RoomTypeCode { get; set; }
        /// <summary>
        /// 房间数
        /// </summary>
        public int NumberOfUnits { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string DistributorReservationId { get; set; }
        /// <summary>
        /// 订单价格
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 入住时间
        /// </summary>
        public string CheckIn { get; set; }
        /// <summary>
        /// 离店时间
        /// </summary>
        public string CheckOut { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 联系人信息
        /// </summary>
        public ContactPersonJsonEt ContactPerson { get; set; }
        /// <summary>
        /// 入住人集合
        /// </summary>
        public List<Guest> GuestList { get; set; }
        /// <summary>
        /// 入住情况信息
        /// </summary>
        public GuestCount GuestCount { get; set; }
    }

    public class ContactPersonJsonEt
    {
        /// <summary>
        /// 联系人名字
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// 联系人姓
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 联系人手机
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 联系人email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 联系人地址
        /// </summary>
        public string Address { get; set; }
    }

    public class Guest
    {
        /// <summary>
        /// 入住人名字
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// 入住人姓
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 入住人手机
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 入住人email
        /// </summary>
        public string Email { get; set; }
    }

    public class GuestCount
    {
        /// <summary>
        /// 成人数
        /// </summary>
        public int AdultCount { get; set; }
        /// <summary>
        /// 儿童数
        /// </summary>
        public int ChildCount { get; set; }
    }
}
