using System;
using System.Collections.Generic;
using System.Text;
using Maticsoft.DBUtility;
using System.Data.Odbc;

namespace Net.Superliujian.AliasDB
{
    public class OdbcDbHelperWrapper : IDbHelperWrapper
    {
        public int ExecuteSql(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOdbc.ExecuteSql(sql, ConvertToMySqlParameter(parameters));
        }

        public System.Data.DataSet Query(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOdbc.Query(sql, ConvertToMySqlParameter(parameters));
        }


        public bool Exists(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOdbc.Exists(sql, ConvertToMySqlParameter(parameters));
        }

        public object GetSingle(string SQLString)
        {
            return DbHelperOdbc.GetSingle(SQLString);
        }


        public void GetColumns(string table, ref List<string> fields, ref string primaryKey)
        {
            DbHelperOdbc.GetColumns(table, ref fields, ref primaryKey);
        }
        private OdbcParameter[] ConvertToMySqlParameter(params CustomParameter[] parameters)
        {
            List<OdbcParameter> list = new List<OdbcParameter>();
            foreach (CustomParameter p in parameters)
            {
                list.Add(new OdbcParameter(p.Name, p.Value));
            }
            return list.ToArray();
        }


    }
}
