using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyPig.Order.Application.Entities.Domestic;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Application.Order;
using FlyPig.Order.Application.Repository.Channel;
using FlyPig.Order.Application.Repository;

namespace FlyPig.Order.Application.Hotel.Channel
{
    public class BigTreeProductChannel : IProductChannel
    {
        private readonly BigTreeRepository bigTreeRepository;
        private readonly HotelRepository hotelRepository;
        public BigTreeProductChannel(ShopType shop)
        {
            Shop = shop;
            bigTreeRepository = new BigTreeRepository(shop);
            hotelRepository = new HotelRepository(Channel);
        }

        public ShopType Shop { get; set; }

        public ProductChannel Channel { get { return ProductChannel.BigTree; } }

        public BookingCheckOutDto BookingCheck(BookingCheckInputDto checkDto)
        {
            var bookCheckResult = new BookingCheckOutDto();
            var roomStatus = bigTreeRepository.GetRoomRateStatus(checkDto.RatePlanId, checkDto.CheckIn, checkDto.CheckOut);
            if (roomStatus.Count > 0)
            {
                List<RateQuotasPrice> rqpList = new List<RateQuotasPrice>();
                foreach (var item in roomStatus)
                {
                    if (item.RoomStatus == 1)
                    {
                        int? price = null;
                        RateQuotasPrice rqp = new RateQuotasPrice();
                        rqp.Date = item.StartDate.ToString("yyyy-MM-dd");
                        if (Shop == ShopType.YinJi)
                        {
                            rqp.Price = (price ?? item.SalePrice + 20) * 100;
                        }
                        else if (Shop == ShopType.LingZhong)
                        {
                            rqp.Price = (price ?? item.SalePrice + 25) * 100;
                        }
                        else if (Shop == ShopType.YinJiGuoLv)
                        {
                            rqp.Price = (price ?? item.SalePrice - 1) * 100;
                        }

                        if (item.Status == 1)
                        {
                            rqp.Quota = 8;
                        }
                        else
                        {
                            bookCheckResult.IsBook = false;
                            bookCheckResult.Message = "房量不足";
                        }
                        rqpList.Add(rqp);
                    }
                    else
                    {
                        bookCheckResult.IsBook = false;
                        bookCheckResult.Message = "满房";
                    }
                }

                if (rqpList.Count > 0)
                {
                    bookCheckResult.DayPrice = rqpList;
                    if (rqpList.Where(u => u.Quota == 0).Count() > 0)
                    {
                        bookCheckResult.IsBook = false;
                        bookCheckResult.Message = "房量不足";
                    }
                    else
                    {
                        bookCheckResult.IsBook = true;
                        bookCheckResult.Message = "检测可预定";
                    }
                }
            }
            else
            {
                bookCheckResult.IsBook = false;
                bookCheckResult.Message = "不存在报价信息";
            }

            return bookCheckResult;
        }

        public TmallHotelPriceInfo GetHotelPriceInfo(BookingCheckInputDto checkDto)
        {
            return bigTreeRepository.GetHotelPriceInfo(checkDto.RoomTypeId, checkDto.RatePlanId, checkDto.CheckIn, checkDto.CheckOut);
        }
    }
}
