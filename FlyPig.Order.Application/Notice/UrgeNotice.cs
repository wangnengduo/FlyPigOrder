using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyPig.Order.Application.Entities.Enum;
using Flypig.Order.Application.Order.Entities;

namespace FlyPig.Order.Application.Order.Notice
{
    public class UrgeNotice : BaseOrderNotice<UrgeRQ, TaoBaoResultXml>
    {
        public UrgeNotice(ShopType shop) : base(shop)
        {
        }

        protected override string ChannelDiscernmentCode => Request.TaoBaoOrderId.ToString();

        protected override TaoBaoResultXml Notice()
        {
            throw new NotImplementedException();
        }
    }
}
