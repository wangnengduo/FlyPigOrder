

using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order.Entities.Dto;
using FlyPig.Order.Application.Entities.Dto;
using FlyPig.Order.Application.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Repository.Order
{
    public interface IOrderRepository
    {
        /// <summary>
        /// 保存订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        ServiceResult SaveOrder(ProductChannel channel, TmallOrderDto order);

        /// <summary>
        /// 是否存在订单
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        bool IsExists(long taobaoOrderId);

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <returns></returns>
        NoticeSimpleOrderDto GetSimpleOrderById(long taobaoOrderId);

        /// <summary>
        /// 更新超时时间
        /// </summary>
        /// <param name="taobaoOrderId"></param>
        /// <param name="delayTime"></param>
        /// <returns></returns>
        bool UpdateOrderDelayTime(long taobaoOrderId, DateTime delayTime);


        /// <summary>
        /// 更新天猫订单通知
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        bool UpdateNoticeOrder(NoticeCommonOrderDto dto);

        /// <summary>
        /// 更新订单备注
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool UpdateOrderRemark(long aid, string remark);

        /// <summary>
        /// 更新发货备注
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool UpdateShipRemark(long aid, int status, string remark);

        /// <summary>
        /// 更新备注及状态
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool UpdateRemarkState(long aid, int status, string remark);

        /// <summary>
        /// 更新订单操作
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool UpdateOrderCaoZuo(long aid, string caozuo);


        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="stutas"></param>
        /// <returns></returns>
        bool UpdateOrderStutas(long aid, int stutas);

        /// <summary>
        /// 更新备注、状态及操作
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        bool UpdateRemarkStateCaoZuo(long aid, int status, string remark, string caozuo);
    }
}
