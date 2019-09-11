using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Framework.Logging
{
    public class LogUtility
    {

        ILog log = null;

        public LogUtility(string logName)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            var fileInfo = new FileInfo(filePath);
            XmlConfigurator.ConfigureAndWatch(fileInfo);
            log = LogManager.GetLogger(logName);
        }

        public static LogUtility Instance
        {
            get
            {
                return new LogUtility("Log");
            }
        }

        public void Info(string message, params object[] agrs)
        {
            if (agrs != null && agrs.Length > 0)
            {
                log.InfoFormat(string.Format(message, agrs));
            }
            else
            {
                log.Info(message);
            }
        }

        public void Error(string message, params object[] agrs)
        {
            if (agrs.Length > 0)
            {
                log.Error(string.Format(message, agrs));
            }
            else
            {
                log.Error(message);
            }
        }

        public void Error(string message, Exception ex)
        {
            log.Error(message, ex);
        }

        public void Debug(string message, params object[] agrs)
        {
            if (agrs.Length > 0)
            {
                log.Debug(string.Format(message, agrs));
            }
            else
            {
                log.Debug(message);
            }
        }
    }
}
