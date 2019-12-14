using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaDuShi.Common
{
    public class JsonUtil
    {
        #region 根据Json字符串转换成指定的实体对象  static T GetObjecByJson<T>(string json)
        /// <summary>
        /// 根据Json字符串转换成指定的实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T GetObjecByJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion

        #region 实体对象转换成Json字符串  static string GetJsonByObj(object obj)
        /// <summary>
        /// 实体对象转换成Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetJsonByObj(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion

        #region DataView 装换成Json  static string DataTable2Json(DataView dv)
        /// <summary>
        /// DataView 装换成Json
        /// </summary>
        /// <param name="dv"></param>
        /// <returns></returns>
        public static string DataTable2Json(DataView dv)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            string str = null;
            try
            {
                jsonBuilder.Append("{");
                jsonBuilder.Append(string.Format("\"count\":\"{0}\"", dv.Count));
                jsonBuilder.Append(string.Format(",\"data\":[", dv.Count));

                var dtCol = dv.Table.Columns;
                for (int i = 0; i < dv.Count; i++)
                {
                    jsonBuilder.Append("{");
                    for (int j = 0; j < dtCol.Count; j++)
                    {
                        jsonBuilder.Append(string.Format("\"{0}\":\"{1}\",", dtCol[j].ColumnName, dv[i][j].ToString()));
                    }
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                    jsonBuilder.Append("},");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("]");
                jsonBuilder.Append("}");
            }
            catch
            {
                return str;
            }

            return jsonBuilder.ToString();
        }
        #endregion
    }
}
