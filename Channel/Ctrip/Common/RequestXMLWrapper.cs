using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ctrip.Common
{
    /// <summary>
    /// 生成xml请求的节点信息 Name(节点名称) AllowEmpty(是否允许空)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RequsetNode : Attribute
    {
        /// <summary>
        /// xml节点名称
        /// </summary>
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }
        /// <summary>
        /// 是否允许空
        /// </summary>
        private bool allowEmpty;
        public bool AllowEmpty
        {
            get
            {
                return allowEmpty;
            }
        }

        public RequsetNode(string name)
        {
            this.name = name;
            this.allowEmpty = true;
        }
        public RequsetNode(string name, bool allowEmpty)
        {
            this.name = name;
            this.allowEmpty = allowEmpty;
        }

        private string attrName;
        public string AttrName
        {
            get { return attrName; }
        }
        public RequsetNode(string name, string attrName, bool allowEmpty)
        {
            this.name = name;
            this.attrName = attrName;
            this.allowEmpty = allowEmpty;
        }

        private int sort;
        public int Sort { get { return sort; } }
        public RequsetNode(string name, int sort)
        {
            this.name = name;
            this.allowEmpty = true;
            this.sort = sort;
        }
        public RequsetNode(string name, bool allowEmpty, int sort)
        {
            this.name = name;
            this.allowEmpty = allowEmpty;
            this.sort = sort;
        }
        public RequsetNode(string name, string attrName, bool allowEmpty, int sort)
        {
            this.name = name;
            this.attrName = attrName;
            this.allowEmpty = allowEmpty;
            this.sort = sort;
        }
    }

    public class RequestXmlWrapper
    {
        public static void ParseCustomAttributes(List<System.Reflection.PropertyInfo> pros, StringBuilder xml, object sender, bool sort = false)
        {
            if (sort)
            {
                pros = pros.OrderBy(c =>
                {
                    int flag = 10000;
                    object[] custattributes = c.GetCustomAttributes(typeof(RequsetNode), true);
                    if (custattributes != null && custattributes.Count() > 0)
                    {
                        RequsetNode n = custattributes[0] as RequsetNode;
                        if (n != null)
                        {
                            return n.Sort;
                        }
                    }
                    return flag;
                }).ToList();
            }
            for (int i = 0; i < pros.Count; i++)
            {
                System.Reflection.PropertyInfo info = pros[i];
                object[] custattributes = info.GetCustomAttributes(typeof(RequsetNode), true);
                if (custattributes != null && custattributes.Count() > 0)
                {
                    RequsetNode n = custattributes[0] as RequsetNode;
                    if (n != null)
                    {
                        if (info.PropertyType.IsClass && !info.PropertyType.Name.Equals("String"))  //是类
                        {
                            if (info.PropertyType.IsArray || info.PropertyType.IsGenericType)
                            {
                                var valList = info.GetValue(sender, null) as System.Collections.IEnumerable;
                                if (valList == null)
                                {
                                    continue;
                                }
                                foreach (object valItem in valList)
                                {
                                    if (valItem.GetType().IsClass && !valItem.GetType().Name.Equals("String"))
                                    {
                                        System.Reflection.PropertyInfo[] lps = valItem.GetType().GetProperties();
                                        if (lps != null && lps.Count() > 0)
                                        {
                                            var children = lps.ToList();
                                            var attrsNodes = GetAttributes(children, n.Name);
                                            if (attrsNodes != null && attrsNodes.Count > 0)
                                            {
                                                ////找出同一个node中的其它attribute
                                                //var nodes = GetAttributes(pros, n.Name);
                                                string temp = string.Format("<{0} ", n.Name);
                                                temp += GetAttributeStr(attrsNodes, valItem, sort);
                                                temp += ">";
                                                xml.Append(temp);
                                                //删除已作attribute的属性，此游标下的属性除外
                                                children.RemoveAll(c => attrsNodes.Contains(c));
                                                ParseCustomAttributes(children, xml, valItem, sort);
                                                xml.AppendFormat("</{0}>", n.Name);
                                            }
                                            else
                                            {
                                                xml.AppendFormat("<{0}>", n.Name);
                                                ParseCustomAttributes(children, xml, valItem, sort);
                                                xml.AppendFormat("</{0}>", n.Name);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        xml.Append(string.Format("<{0}>{1}</{0}>", n.Name, valItem.ToString()));
                                    }
                                }
                            }
                            else
                            {
                                object tvp = info.GetValue(sender, null);
                                if (tvp != null)
                                {

                                    System.Reflection.PropertyInfo[] ls = tvp.GetType().GetProperties();
                                    if (ls != null && ls.Count() > 0)
                                    {
                                        var children = ls.ToList();
                                        var attrsNodes = GetAttributes(children, n.Name);
                                        if (attrsNodes != null && attrsNodes.Count > 0)
                                        {
                                            ////找出同一个node中的其它attribute
                                            //var nodes = GetAttributes(pros, n.Name);
                                            string temp = string.Format("<{0} ", n.Name);
                                            temp += GetAttributeStr(attrsNodes, tvp, sort);
                                            temp += ">";
                                            xml.Append(temp);
                                            //删除已作attribute的属性，此游标下的属性除外
                                            children.RemoveAll(c => attrsNodes.Contains(c));
                                            ParseCustomAttributes(children, xml, tvp, sort);
                                            xml.AppendFormat("</{0}>", n.Name);
                                        }
                                        else
                                        {
                                            xml.AppendFormat("<{0}>", n.Name);
                                            ParseCustomAttributes(children, xml, tvp, sort);
                                            xml.AppendFormat("</{0}>", n.Name);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!n.AllowEmpty)
                                    {
                                        throw new Exception(string.Format("属性{0}值不能为空！", info.Name));
                                    }
                                }
                            }
                        }
                        else
                        {
                            object val = info.GetValue(sender, null);
                            if (!n.AllowEmpty)//不允许空
                            {
                                if (val == null || (val != null && string.IsNullOrEmpty(val.ToString())))
                                {
                                    throw new Exception(string.Format("属性{0}值不能为空！", info.Name));
                                }
                            }
                            if (val != null && !string.IsNullOrEmpty(val.ToString()))
                            {
                                if (!string.IsNullOrEmpty(n.AttrName))
                                {
                                    //找出同一个node中的其它attribute
                                    var nodes = GetAttributes(pros, n.Name);
                                    string temp = string.Format("<{0} ", n.Name);
                                    temp += GetAttributeStr(nodes, sender, sort);
                                    temp += string.Format("/>", n.Name);
                                    xml.Append(temp);
                                    //删除已作attribute的属性，此游标下的属性除外
                                    pros.RemoveAll(c => nodes.Contains(c) && c != info);

                                }
                                else
                                {
                                    xml.Append(string.Format("<{0}>{1}</{0}>", n.Name, val.ToString()));
                                }

                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 同一个node下有attribute的属性
        /// </summary>
        private static List<System.Reflection.PropertyInfo> GetAttributes(List<System.Reflection.PropertyInfo> pros, string nodeName)
        {
            List<System.Reflection.PropertyInfo> result = new List<System.Reflection.PropertyInfo>();
            foreach (var info in pros)
            {
                object[] custattributes = info.GetCustomAttributes(typeof(RequsetNode), true);
                if (custattributes != null && custattributes.Count() > 0)
                {
                    RequsetNode n = custattributes[0] as RequsetNode;
                    if (n != null && n.Name == nodeName && !string.IsNullOrEmpty(n.AttrName))
                    {
                        result.Add(info);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// attribute字符串
        /// </summary>
        private static string GetAttributeStr(List<System.Reflection.PropertyInfo> list, object sender, bool sort = false)
        {
            StringBuilder sb = new StringBuilder(50);
            if (sort)
            {
                list = list.OrderBy(c =>
                {
                    int flag = 10000;
                    object[] custattributes = c.GetCustomAttributes(typeof(RequsetNode), true);
                    if (custattributes != null && custattributes.Count() > 0)
                    {
                        RequsetNode n = custattributes[0] as RequsetNode;
                        if (n != null)
                        {
                            return n.Sort;
                        }
                    }
                    return flag;
                }).ToList();
            }
            foreach (var info in list)
            {
                object[] custattributes = info.GetCustomAttributes(typeof(RequsetNode), true);
                if (custattributes != null && custattributes.Count() > 0)
                {
                    RequsetNode n = custattributes[0] as RequsetNode;
                    if (n != null)
                    {
                        object val = info.GetValue(sender, null);
                        if (!n.AllowEmpty)//不允许空
                        {
                            if (val == null || (val != null && string.IsNullOrEmpty(val.ToString())))
                            {
                                throw new Exception(string.Format("属性{0}值不能为空！", info.Name));
                            }
                        }
                        if (val != null && !string.IsNullOrEmpty(val.ToString()))
                        {
                            sb.AppendFormat("{0}=\"{1}\" ", n.AttrName, val.ToString());
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public static string ToXml(object obj, bool sort = false)
        {
            StringBuilder xml = new StringBuilder(200);
            System.Reflection.PropertyInfo[] pros = obj.GetType().GetProperties();
            ParseCustomAttributes(pros.ToList(), xml, obj, sort);
            return xml.ToString();
        }

    }

    public class XmlParse
    {
        public static void SelectSingleNode(XmlNode node, string xpath, Action<XmlNode> callback)
        {
            if (node != null)
            {
                XmlNode tmp = node.SelectSingleNode(xpath);
                if (tmp != null && callback != null)
                {
                    callback(tmp);
                }
            }
        }

        public static void SelectNodes(XmlNode node, string xpath, Action<XmlNodeList> callback)
        {
            if (node != null)
            {
                XmlNodeList tmp = node.SelectNodes(xpath);
                if (tmp != null && callback != null)
                {
                    callback(tmp);
                }
            }
        }

        public static string GetAttribute(XmlNode node, string name)
        {
            if (node != null)
            {
                XmlAttribute attribute = node.Attributes[name];
                if (attribute != null)
                {
                    return attribute.Value;
                }
            }
            return string.Empty;
        }
    }

    public static class StringHelper
    {
        public static int ToInt32(this string value, int defaultValue = int.MinValue)
        {
            if (!string.IsNullOrEmpty(value))
            {
                int temp = 0;
                if (int.TryParse(value, out temp))
                {
                    return temp;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }

        }

        public static double ToDouble(this string value, double defaultValue = double.MinValue)
        {
            if (!string.IsNullOrEmpty(value))
            {
                double temp = 0;
                if (double.TryParse(value, out temp))
                {
                    return temp;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }

        }

        public static decimal ToDecimal(this string value, decimal defaultValue = decimal.MinValue)
        {
            if (!string.IsNullOrEmpty(value))
            {
                decimal temp = 0;
                if (decimal.TryParse(value, out temp))
                {
                    return temp;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }

        }

        public static float ToFloat(this string value, float defaultValue = float.MinValue)
        {
            if (!string.IsNullOrEmpty(value))
            {
                float temp = 0;
                if (float.TryParse(value, out temp))
                {
                    return temp;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }

        }

        public static DateTime ToDateTime(this string value, DateTime defaultValue = new DateTime())
        {
            if (!string.IsNullOrEmpty(value))
            {
                DateTime temp = new DateTime();
                if (DateTime.TryParse(value, out temp))
                {
                    return temp;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }

        }

        public static bool ToBoolean(this string value, bool defaultValue = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                bool temp = false;
                if (bool.TryParse(value, out temp))
                {
                    return temp;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
