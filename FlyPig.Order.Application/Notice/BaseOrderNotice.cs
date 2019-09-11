using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FlyPig.Order.Application.Common.Tmall;
using FlyPig.Order.Application.Entities;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Hotel.Channel;
using FlyPig.Order.Application.MT.Order;
using FlyPig.Order.Application.MT.Order.Channel;
using FlyPig.Order.Application.Repository.Order;
using SqlSugar;
using Flypig.Order.Application.Order.Entities;
using FlyPig.Order.Framework.Logging;
using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Framework.Common;

namespace FlyPig.Order.Application.Order.Notice
{
    /// <summary>
    /// 飞猪订单通知基类
    /// </summary>
    public abstract class BaseOrderNotice<TRequest, TResponse> : IOrderNotice
        where TRequest : TaoBaoXml, new()
        where TResponse : TaoBaoResultXml, new()
    {

        protected IProductChannel productChannel;
        public IOrderRepository orderRepository;
        protected readonly TmallApiClient tmallClient;
        protected readonly LogWriter requestLogWriter;
        protected readonly AliTripOrderClient aliTripOrderClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shop"></param>
        public BaseOrderNotice(ShopType shop)
        {
            this.Shop = shop;
            tmallClient = new TmallApiClient(shop);
            //productChannel = ProductChannelFactory.CreateChannel(Channel, shop);
          //  orderRepository = ProductChannelFactory.CreateOrderRepository(Shop, Channel);
            requestLogWriter = new LogWriter("Tmall/Request");
            aliTripOrderClient = new AliTripOrderClient(shop);
        }

        protected TRequest Request { get; set; }

        public ShopType Shop { get; set; }

        public ProductChannel Channel { get; set; }

        /// <summary>
        /// 渠道识别编码
        /// </summary>
        protected abstract string ChannelDiscernmentCode { get; }

        protected abstract TResponse Notice();

        public TaoBaoResultXml ExcuteOrder(string xml)
        {
            this.Request = XmlUtility.Deserialize<TRequest>(xml);
            var result = new TResponse();
            try
            {
                InitChannel();    //初始化渠道信息
            }
            catch
            {
                result.ResultCode = "0";
                result.Message = "订单不存在";
                return result;
            }

            //校验接口账号密码
            if (!ValidaeUser(Request.AuthenticationToken))
            {
                result.Message = "接口账号密码校验失败";
                result.ResultCode = "-998";
                return result;
            }

            result = Notice();
            return result;
        }

        #region 校验用户
        /// <summary>
        /// 校验用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool ValidaeUser(AuthenticationToken at)
        {
            if (at == null)
                return false;

            if (at.Username.Equals(tmallClient.UserName) && at.Password.Equals(tmallClient.Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 初始化渠道

        public void InitChannel()
        {
            var markType = GetMarkType();
            InitChannel(ChannelDiscernmentCode, markType);
        }


        /// <summary>
        /// 获取订单ID标识类型
        /// </summary>
        /// <returns></returns>
        private OrderMarkType GetMarkType()
        {
            Type t = typeof(TRequest);
            switch (t.Name)
            {
                case "OrderRefundRQ":
                case "CancelRQ":
                case "PaySuccessRQ":
                case "QueryStatusRQ":
                    return OrderMarkType.OrderId;
                case "BookRQ":
                case "ValidateRQ":
                    return OrderMarkType.RatePlanCode;
                default: return OrderMarkType.OrderId;
            }
        }

        /// <summary>
        /// 初始化渠道
        /// </summary>
        /// <param name="code"></param>
        /// <param name="markType"></param>
        protected void InitChannel(string code, OrderMarkType markType)
        {
            if (markType == OrderMarkType.OrderId)
            {
                Channel = AliTripOrderService.GetChannelByOrderId(code);
            }
            else
            {
                Channel = AliTripOrderService.GetChannelByRPCode(code);
            }

            productChannel = ProductChannelFactory.CreateChannel(Channel, Shop);
            orderRepository = ProductChannelFactory.CreateOrderRepository(Shop, Channel);
        }


        #endregion

        #region 获取酒店ID
        /// <summary>
        /// 获取酒店ID
        /// </summary>
        /// <param name="rpCode"></param>
        /// <returns></returns>
        public HotelIdDto GetIdInfoByRpCode(string rpCode)
        {
            try
            {
                var idDto = new HotelIdDto();
                rpCode = Regex.Replace(rpCode, "[A-Za-z]+", "");
                var hotelInfo = rpCode.Split('_');
                idDto.HotelId = hotelInfo[0];
                idDto.RoomTypeId = hotelInfo[1];
                idDto.RatePlanId = hotelInfo[2];
                return idDto;
            }
            catch
            {
                return null;
            }
        }
        #endregion

    }
}
