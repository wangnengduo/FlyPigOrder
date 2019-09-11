using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flypig.Order.Application.Order.Entities.Dto
{
    public class NoticeCommonOrderDto
    {
        public NoticeOrderOperatType OperatType { get; set; }

        public long TaoBaoOrderId { get; set; }

        public int State { get; set; }

        public string Remark { get; set; }

        public string CaoZuo { get; set; }
    }


    public class NoticePayOrderDto : NoticeCommonOrderDto
    {
        public string AlipayTradeNo { get; set; }

        public string TradeStatus { get; set; }

        public decimal AlipayPay { get; set; }

    }

    public class NoticeRefundOrderDto : NoticeCommonOrderDto
    {
        public int Refuse { get; set; }
    }

    public class NoticeCancelOrderDto : NoticeCommonOrderDto
    {
        public string Reason { get; set; }
    }


    public enum NoticeOrderOperatType
    {
        Pay = 1,
        Refund = 2,
        Cancel = 3,
        NotCenter = 4
    }
}
