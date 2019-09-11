using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlyPig.Order.Framework.Common
{
    public class XmlUtility
    {

        public static T Deserialize<T>(string xml)
        where T : class
        {
            try
            {
                StringReader sr = new StringReader(xml);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T obj = serializer.Deserialize(sr) as T;
                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
