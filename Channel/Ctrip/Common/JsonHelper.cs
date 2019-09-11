using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CtripApi.Common
{
    /// <summary>
    /// Json帮助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string SerializeObject(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        /// <summary>
        /// 将对象序列化为JSON格式
        /// </summary>
        /// <param name="o">对象</param>
        /// <returns>json字符串</returns>
        public static string SerializeIndented(object o)
        {
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string json = JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static string GetJsonValue(string json, string path, bool isValueType = false)
        {
            var obj = JObject.Parse(json);
            var node = obj.SelectToken(path);
            if (node == null || string.IsNullOrEmpty(node.ToString()))
            {
                return isValueType ? "0" : "";
            }

            return node.ToString();
        }

        public static IEnumerable<JToken> GetJosnNodes(string json, string path)
        {
            var obj = JObject.Parse(json);
            var jTokes = obj.SelectTokens(path);
            return jTokes;
        }

        public static JArray GetJosnArray(string json)
        {
            var array = JArray.Parse(json);

            return array;
        }


        public static IEnumerable<JToken> GetJosnNodes(JToken jtoken, string path)
        {
            var jTokes = jtoken.SelectTokens(path);
            return jTokes;
        }

        public static string GetJsonValue(JToken jToken, string path, bool isValueType = false)
        {
            var token = jToken.SelectToken(path);

            if (token == null)
            {
                return isValueType ? "0" : "";
            }

            string value = token.ToString();
            return value == "[]" ? isValueType ? "0" : "" : value;
        }

        /// <summary>
        /// 解析JSON字符串生成对象实体
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        /// <returns>对象实体</returns>
        public static T DeserializeJsonToObject<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = o as T;
            return t;
        }

        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }

        /// <summary>
        /// 反序列化JSON到给定的匿名对象.
        /// </summary>
        /// <typeparam name="T">匿名对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="anonymousTypeObject">匿名对象</param>
        /// <returns>匿名对象</returns>
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }

        public static string Searilize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }


        public static string Searilize(string format, Type type, object value)
        {
            return Searilize(format, type, value);
        }


        public static string Searilize(string format, Type type, object value, Type extendType)
        {
            string str = "";
            if (format == "xml")
            {
                StringWriter sw = new StringWriter();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                //ns.Add("", "");

                System.IO.Stream stream = new System.IO.MemoryStream();

                System.Xml.Serialization.XmlSerializer serializer = null;
                if (extendType == null)
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(type);
                }
                else
                {
                    serializer = new System.Xml.Serialization.XmlSerializer(type, new Type[] { extendType });
                }
                serializer.Serialize(stream, value, ns);

                stream.Seek(0, System.IO.SeekOrigin.Begin);
                System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);

                str = reader.ReadToEnd();

            }
            else
            {

                var enumConverter = new Newtonsoft.Json.Converters.StringEnumConverter();
                var settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
                settings.Converters.Add(enumConverter);
                str = JsonConvert.SerializeObject(value, settings);
            }

            return str;
        }

        public static T Deserialize<T>(string format, string xml)
        {
            if (format == "xml")
            {


                T t = default(T);
                XmlSerializer xs = new XmlSerializer(typeof(T));
                //MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xml));
                //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                //t = xs.Deserialize(memoryStream);

                //using (StringReader rdr = new StringReader(xml))
                //{
                //    t = (T)xs.Deserialize(rdr);
                //}




                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                var rd = new XmlNodeReader(doc.DocumentElement);
                t = (T)xs.Deserialize(rd);

                //XElement root = XElement.Parse(xml); 

                return t;
            }
            if (xml.StartsWith("{") || xml.StartsWith("<"))
            {
                return JsonConvert.DeserializeObject<T>(xml);
            }
            else
            {
                return default(T);
            }
        }

        private static String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
    }
}