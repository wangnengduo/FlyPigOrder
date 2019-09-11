using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Application.Common.Tmall
{
    public class AliTripConfig
    {
        public int Id { get; set; }
        public string AppName { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string SessionKey { get; set; }
        public string RefreshToken { get; set; }
        public int ShopType { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime ModifyTime { get; set; }
    }
}
