using FlyPig.Order.Framework.HttpWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Framework.Common
{
    public class SMSUilitily
    {
        private const string SN = "SDK-BBX-010-28331";
        private const string Password = "bf08$-fdb8-";
        private const string Url = "http://sdk.entinfo.cn:8061/webservice.asmx";
        private const string ReplaceUrl = "http://sdk2.entinfo.cn:8061/webservice.asmx";     //替代Url

        public static bool Send(int channel, string tell, string content)
        {
            try
            {
                string url = "http://admin.wzlzly.com/Service/SendMessage.ashx";
                string data = string.Format("channel={0}&tell={1}&content={2}", channel, tell, content);
                string result = WebHttpRequest.Post(url, data);
                if (result == "0")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 发送盛旅短信
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SendShengLv(string phone, string content)
        {
            string result = string.Empty;
            try
            {
                string pwd = string.Concat(SN, Password).GetMD5().ToUpper();
                string url = string.Format("{0}/mdsmssend?sn={1}&pwd={2}&mobile={3}&content={4}&ext=&stime=&rrid=&msgfmt=", Url, SN, pwd, phone, content);
                result = WebHttpRequest.Get(url);
                return result;
            }
            catch (Exception ex)
            {
              //  logWriter.Write(ex.ToString());
                return "-1";
            }
            finally
            {
                MessageRecord mr = new MessageRecord();
                mr.Phone = phone;
                mr.Content = content;
                mr.Result = result;
                mr.CreateTime = DateTime.Now;
                //TODO  保存短信记录
            }
        }
    }



    ///<summary>
    ///
    ///</summary>
    public class MessageRecord
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Phone { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Content { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Result { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? CreateTime { get; set; }

    }
}
