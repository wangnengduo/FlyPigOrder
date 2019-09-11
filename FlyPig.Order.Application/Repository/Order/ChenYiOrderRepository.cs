using FlyPig.Order.Application.Entities;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Repository.Order
{
    public class ChenYiOrderRepository
    {

        public bool UpdateCancelRemark(long taobaoOrderId)
        {
            string sql = string.Format("update dingdan_info set beizhu=('【XL】:订单申请取消  [{1}]<br/>'+beizhu) where fax='{0}'", taobaoOrderId, DateTime.Now.ToString());
            return SqlSugarContext.TravelskyInstance.Ado.ExecuteCommand(sql) > 0;
        }


        public dingdan_info GetOrderByTaoBaoOrderId(long taobaoOrderId)
        {
            return SqlSugarContext.TravelskyInstance.Queryable<dingdan_info>().Where(u => u.fax == taobaoOrderId.ToString()).First();
        }
    }

}
