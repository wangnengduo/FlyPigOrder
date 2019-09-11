using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ctrip.Response
{
    [Serializable]
    public abstract class BaseResponse
    {
        /// <summary>
        /// 原始内容
        /// </summary>
        public string ResponseContent { get; set; }
        public Error Error { get; set; }
        /// <summary>
        /// 加载的xml
        /// </summary>
        public System.Xml.XmlDocument ResponseXml { get; internal set; }
        /// <summary>
        /// 结果数据xml
        /// </summary>
        public System.Xml.XmlDataDocument ResultXML { get; set; }

        public Exception Exceptions { get; set; }

        internal abstract void ParseXml();
    }
    [Serializable]
    public class Error
    {
        /// <summary>
        /// 在返回错误是才出现的Error节点
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 在返回错误是才出现的Error节点
        /// </summary>
        public string Message { get; set; }

        public string Type { get; set; }
    }
}
