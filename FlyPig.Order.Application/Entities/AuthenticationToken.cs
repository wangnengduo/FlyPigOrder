using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Flypig.Order.Application.Order.Entities
{
    [Serializable]
    public class AuthenticationToken
    {
        [XmlElement]
        public string Username { get; set; }
        [XmlElement]
        public string Password { get; set; }
        [XmlElement]
        public string CreateToken { get; set; }
    }
}
