using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyPig.Order.Application.Common;
using FlyPig.Order.Core;
using FlyPig.Order.Core.Model;
using SqlSugar;

namespace FlyPig.Order.Application.Repository.Order
{
    /// <summary>
    /// 盛旅天猫订单仓储层
    /// </summary>
    public class ShengLvOrderRepository : ThirdOrderRepository
    {
        public ShengLvOrderRepository() : base(SqlSugarContext.ResellbaseInstance)
        {
        }

        public ShengLvOrderRepository(SqlSugarClient sqlSugar) : base(sqlSugar)
        {
        }

        public bool UpdateCancelRemark(long taobaoOrderId)
        {
            string sql = string.Format("update dingdan_info set beizhu=(beizhu + '【AY】:订单申请取消  [{1}]<br/>') where fax='{0}'", taobaoOrderId, DateTime.Now.ToString());
            return SqlSugarContext.ResellbaseInstance.Ado.ExecuteCommand(sql) > 0;
        }
    }
}
