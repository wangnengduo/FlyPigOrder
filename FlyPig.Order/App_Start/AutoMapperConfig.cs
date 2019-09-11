using AutoMapper;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlyPig.Order
{
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.Reset();
            Mapper.Initialize(config =>
            {
                config.CreateMap(typeof(TmallOrderDto), typeof(TB_hotelcashorder));
                config.CreateMap(typeof(TmallOrderDto), typeof(tb_hotelorder));
            });
        }
    }
}