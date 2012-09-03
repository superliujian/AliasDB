using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Maticsoft.DBUtility;

namespace Net.Superliujian.AliasDB
{
    /// <summary>
    /// 自适应的MySQL数据实体类
    /// 可以忽略数据库的改变而不用改变数据库访问接口
    /// 需要获取描述表信息
    /// </summary>
    public class DataAccessSQLServer : DataAccessBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table">不包含表前缀的表名</param>
        public DataAccessSQLServer(string table)
            : base(table)
        {
        }


        protected override void InitializeDataAdaptor()
        {
            DbHelperWrapper = new SQLDbHelperWrapper();
        }


    }
}
