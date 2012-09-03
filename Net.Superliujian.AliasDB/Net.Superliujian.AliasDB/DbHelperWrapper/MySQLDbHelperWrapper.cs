using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using Maticsoft.DBUtility;

namespace Net.Superliujian.AliasDB
{
    public class MySQLDbHelperWrapper : IDbHelperWrapper
    {
        public int ExecuteSql(string sql, params CustomParameter[] parameters)
        {
            return DbHelperMySQL.ExecuteSql(sql, ConvertToMySqlParameter(parameters));
        }

        public System.Data.DataSet Query(string sql, params CustomParameter[] parameters)
        {
            return DbHelperMySQL.Query(sql, ConvertToMySqlParameter(parameters));
        }


        public bool Exists(string sql, params CustomParameter[] parameters)
        {
            return DbHelperMySQL.Exists(sql, ConvertToMySqlParameter(parameters));
        }



        public object GetSingle(string SQLString)
        {
            return DbHelperMySQL.GetSingle(SQLString);
        }

        public void GetColumns(string table, ref List<string> fields, ref string primaryKey)
        {
            DbHelperMySQL.GetColumns(table, ref fields, ref primaryKey);
        }

        private MySqlParameter[] ConvertToMySqlParameter(params CustomParameter[] parameters)
        {
            List<MySqlParameter> list = new List<MySqlParameter>();
            foreach (CustomParameter p in parameters)
            {
                list.Add(new MySqlParameter(p.Name, p.Value));
            }
            return list.ToArray();
        }




       
    }
}
