using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Framework.Logging
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public class LogWriter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LogWriter() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path"></param>
        public LogWriter(string path)
        {
            this.Path = path;
        }

        /// <summary>
        /// 保存路径
        /// </summary>
        public string Path { private set; get; }

        public void WriteOrder(string orderId, string msg, params object[] args)
        {
            string message = string.Format(msg, args);
            WriteOrder(orderId, message);
        }

        /// <summary>
        /// 写入订单日志
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="msg"></param>
        public void WriteOrder(string orderId, string msg)
        {
            try
            {
                string path = ConfigurationManager.AppSettings["WinServer_Log"];
                if (string.IsNullOrEmpty(path))
                {
                    path = System.AppDomain.CurrentDomain.BaseDirectory;
                }

                path = System.IO.Path.Combine(path, Path + "\\" + DateTime.Now.ToString("yyyy-MM-dd"));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string logtxt = string.Concat(path, "\\", orderId, ".log");
                if (File.Exists(@logtxt))
                {
                    using (FileStream logFile = new FileStream(@logtxt, FileMode.Append))
                    {
                        WriteLog(logFile, msg, null);
                    }
                }
                else
                {
                    using (FileStream logFile = new FileStream(@logtxt, FileMode.Create))
                    {
                        WriteLog(logFile, msg, null);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void Write(string msg, params object[] args)
        {
            string message = string.Empty;
            if (args.Length > 0)
            {
                message = string.Format(msg, args);
            }
            else
            {
                message = msg;
            }
            WriteException(message, null);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="ex">错误日志异常</param>
        public void WriteException(string msg, Exception ex)
        {
            try
            {
                string path = ConfigurationManager.AppSettings["WinServer_Log"];
                if (string.IsNullOrEmpty(path))
                {
                    path = System.AppDomain.CurrentDomain.BaseDirectory;
                }

                path = System.IO.Path.Combine(path, Path);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string logtxt = string.Concat(path, "\\", DateTime.Now.ToString("yyyy-MM-dd"), ".log");
                if (File.Exists(@logtxt))
                {
                    using (FileStream logFile = new FileStream(@logtxt, FileMode.Append))
                    {
                        WriteLog(logFile, msg, ex);
                    }
                }
                else
                {
                    using (FileStream logFile = new FileStream(@logtxt, FileMode.Create))
                    {
                        WriteLog(logFile, msg, ex);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        private void WriteLog(FileStream logFile, string msg, Exception ex)
        {
            StreamWriter sw = new StreamWriter(logFile);
            sw.WriteLine(string.Format("[日志]: {0} - {1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "--->", msg));
            if (ex != null)
            {
                sw.WriteLine(string.Format(ex.ToString()));
            }
            sw.Dispose();
            sw.Close();
        }
    }
}
