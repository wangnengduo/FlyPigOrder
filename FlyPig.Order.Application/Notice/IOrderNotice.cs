using Flypig.Order.Application.Order.Entities;

namespace FlyPig.Order.Application.Order.Notice
{
    public interface IOrderNotice
    {

        TaoBaoResultXml ExcuteOrder(string xml);

    }

}
