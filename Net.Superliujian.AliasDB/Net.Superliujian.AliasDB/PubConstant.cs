using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Net.Superliujian.AliasDB
{
    /// <summary>
    /// 通过配置文件获取的配置参数
    /// 
    /// Modified by LiuJian
    /// Name is the same as Maticsoft.DBUtility.PubConstant
    ///  <appSettings>
    ///    <add key="ConnectionString" value=""/>
    ///    <add key="TablePrefix" value=""/>
    ///  </appSettings>
    /// </summary>
    public static class PubConstant
    {
        private static Dictionary<string, string> _Cache = new Dictionary<string, string>();

        private static string GetFromCache(string key)
        {
            if (_Cache.ContainsKey(key))
                return _Cache[key];
            else
                return GetDirectly(key);
        }

        private static string GetDirectly(string key)
        {
            string value = "";
            try
            {
                value = ConfigurationManager.AppSettings[key].ToString();
            }
            catch { }
            _Cache[key] = value;
            return value;
        }

        /// <summary>
        /// 连接字符串
        /// MySQL: Server=localhost;Database=bichicun;Uid=root;Pwd=123456;
        /// SQLite:Data Source=db_file
        /// SQL:Server=localhost\SQLEXPRESS2008R2;Trusted_Connection=True;Database=test;
        /// OleDB:Provider=SQLNCLI;Server=localhost\SQLEXPRESS2008R2;Database=test;UID=sa;PWD=QIUmei1202@;//SQL Server
        ///       Provider=MySQLProv;Data Source=mydb;User Id=myUsername;Password=myPassword;//MySQL
        /// </summary>
        public static string ConnectionString
        {
            get { return GetFromCache("ConnectionString"); }
        }

        /// <summary>
        /// 数据库表的前缀
        /// </summary>
        public static string TablePrefix
        {
            get { return GetFromCache("TablePrefix"); }
        }
    }
}
