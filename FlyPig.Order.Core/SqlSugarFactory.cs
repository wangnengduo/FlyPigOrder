using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyPig.Order.Core
{
    /// <summary>
    /// SqlSugar 工厂类
    /// </summary>
    public class SqlSugarFactory
    {
        public static SqlSugarClient GetSugarClient(string connKey, string sugarConnection)
        {
            string conn = string.Empty;
            try
            {
                conn = ConfigurationManager.ConnectionStrings[connKey].ConnectionString;
                if (string.IsNullOrEmpty(conn))
                {
                    conn = sugarConnection;
                }
            }
            catch
            {
                conn = sugarConnection;
            }

            var connectionConfig = new ConnectionConfig
            {
                DbType = DbType.SqlServer,
                ConnectionString = conn,
                IsAutoCloseConnection = true
            };

            return new SqlSugarClient(connectionConfig);
        }
    }
}
