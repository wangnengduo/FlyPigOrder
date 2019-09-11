using FlyPig.Order.Application.Common;
using FlyPig.Order.Application.Entities.Domestic;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Application.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Hotel.Channel
{
    public interface IProductChannel
    {
        /// <summary>
        /// 下单校验
        /// </summary>
        /// <param name="checkDto"></param>
        /// <returns></returns>
        BookingCheckOutDto BookingCheck(BookingCheckInputDto checkDto);

        /// <summary>
        /// 获取酒店价格
        /// </summary>
        /// <returns></returns>
        TmallHotelPriceInfo GetHotelPriceInfo(BookingCheckInputDto checkDto);


    }
}
