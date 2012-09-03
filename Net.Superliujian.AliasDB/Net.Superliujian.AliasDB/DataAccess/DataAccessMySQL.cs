using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using Maticsoft.DBUtility;
namespace Net.Superliujian.AliasDB
{
    /// <summary>
    /// 自适应的MySQL数据实体类
    /// 可以忽略数据库的改变而不用改变数据库访问接口
    /// 需要获取描述表信息
    /// 
    /// </summary>
    public class DataAccessMySQL : DataAccessBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table">不包含表前缀的表名</param>
        public DataAccessMySQL(string table)
            : base(table)
        {
        }

        public override string NameQuotedLeft
        {
            get
            {
                return "`";
            }
        }

        public override string NameQuotedRight
        {
            get
            {
                return "`";
            }
        }
       
        protected override void InitializeDataAdaptor()
        {
            ParameterSeperator = "?";
            DbHelperWrapper = new MySQLDbHelperWrapper();
        }
    }
}
