using Flypig.Order.Application.Common;
using Flypig.Order.Application.Order;
using FlyPig.Order.Application.Entities.Enum;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Channel.BigTree
{
    public class BigtreeOrderChannel : IOrderChannel
    {
        public ServiceResult CancelOrder(int id)
        {
            var result = new ServiceResult();
            return result;
        }

        public ServiceResult CreateOrder(int id)
        {
            var result = new ServiceResult();
            var order = SqlSugarContext.BigTreeInstance.Queryable<tb_hotelorder>().Where(u => u.aId == id).First();
            if (order == null)
            {
                return result.SetError("下单失败，订单不存在");
            }

            if (CheckIsAutoConfirm((int)order.hotelID, (int)order.roomID, order.roomNum, order.checkInDate, order.checkOutDate))
            {
                order.caozuo = $"{order.caozuo}system 标识订单为即时确认订单 [{DateTime.Now.ToString()}]\n";
                order.remark = $"{order.remark}【系统】此单为即时确认订单 [{DateTime.Now.ToString()}]<br/>";
                SqlSugarContext.BigTreeInstance.Updateable(order).UpdateColumns(u => new { u.caozuo, u.remark }).ExecuteCommand();

                AliTripOrderService aliTripOrderService = new AliTripOrderService((ShopType)order.shopType);
                var updateStr = aliTripOrderService.UpdateOrderStatus(order.taoBaoOrderId, 6, 1);

                return result.SetSucess("下单成功");
            }
            else
            {
                return result.SetError("下单失败，非即时确认");
            }
        }

        private static bool CheckIsAutoConfirm(int hid, int rid, int roomnum, DateTime indate, DateTime outdate)
        {
            if (DateTime.Now.Date.CompareTo(indate.Date) > 0)
            {
                return false;
            }

            var roomInfo = SqlSugarContext.BigTreeInstance.Queryable<hotelRooms>().Where(u => u.hId == hid && u.rId == rid).First();
            if ((DateTime.Now.Hour >= roomInfo.lastConfirm && indate.Date.CompareTo(DateTime.Now.Date) == 0))
            {
                return false;
            }

            var instantRoomList = SqlSugarContext.BigTreeInstance.Queryable<instantRoom>().Where(u => u.hid == hid && u.roomid == rid && u.date >= indate && u.date < outdate).ToList();
            TimeSpan sp = outdate.Subtract(indate);

            for (int i = 0; i < sp.Days; i++)
            {
                DateTime currentTime = indate.AddDays(i);
                int num = 0;
                var instantRoom = instantRoomList.Where(u => u.date == currentTime).FirstOrDefault();
                if (instantRoom == null)
                {
                    return false;
                }

                if (roomnum > num && DateTime.Now.Hour >= 23)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
