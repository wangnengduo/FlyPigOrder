using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Framework.HttpWeb
{
    public class StreamUtility
    {
        /// <summary>
        /// 将流转换成字符串
        /// </summary>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public static string GetString(Stream stream)
        {
            return GetString(stream, Encoding.Default);
        }

        /// <summary>
        /// 将流转换成字符串
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encoding">编码类型</param>
        /// <returns></returns>
        public static string GetString(Stream stream, Encoding encoding)
        {
            using (StreamReader sr = new StreamReader(stream, encoding))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
