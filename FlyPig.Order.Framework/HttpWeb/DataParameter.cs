using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyPig.Order.Framework.HttpWeb
{
    public class DataParameter
    {
        public IDictionary<string, string> ParamList = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            var param = new KeyValuePair<string, string>(key, value);
            ParamList.Add(param);
        }

        public string GetDataParam()
        {
            StringBuilder sbParam = new StringBuilder();
            foreach (var item in ParamList)
            {
                sbParam.Append(string.Format("{0}={1}&", item.Key, item.Value));
            }
            sbParam.Remove(sbParam.Length - 1, 1);
            return sbParam.ToString();
        }
    }
}
