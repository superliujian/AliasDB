using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Maticsoft.DBUtility;

namespace Net.Superliujian.AliasDB
{
    public class SQLDbHelperWrapper : IDbHelperWrapper
    {
        public int ExecuteSql(string sql, params CustomParameter[] parameters)
        {
            return DbHelperSQL.ExecuteSql(sql, ConvertToMySqlParameter(parameters));
        }

        public System.Data.DataSet Query(string sql, params CustomParameter[] parameters)
        {
            return DbHelperSQL.Query(sql, ConvertToMySqlParameter(parameters));
        }


        public bool Exists(string sql, params CustomParameter[] parameters)
        {
            return DbHelperSQL.Exists(sql, ConvertToMySqlParameter(parameters));
        }

        public object GetSingle(string SQLString)
        {
            return DbHelperSQL.GetSingle(SQLString);
        }

        public void GetColumns(string table, ref List<string> fields, ref string primaryKey)
        {
            DbHelperSQL.GetColumns(table, ref fields, ref primaryKey);
        }

        private SqlParameter[] ConvertToMySqlParameter(params CustomParameter[] parameters)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            foreach (CustomParameter p in parameters)
            {
                list.Add(new SqlParameter(p.Name, p.Value));
            }
            return list.ToArray();
        }


    }
}
