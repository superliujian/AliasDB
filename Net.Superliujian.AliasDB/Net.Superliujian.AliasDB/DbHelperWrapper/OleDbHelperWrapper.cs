using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using Maticsoft.DBUtility;

namespace Net.Superliujian.AliasDB
{
    public class OleDbHelperWrapper : IDbHelperWrapper
    {
        public int ExecuteSql(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOleDb.ExecuteSql(sql, ConvertToMySqlParameter(parameters));
        }

        public System.Data.DataSet Query(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOleDb.Query(sql, ConvertToMySqlParameter(parameters));
        }


        public bool Exists(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOleDb.Exists(sql, ConvertToMySqlParameter(parameters));
        }

        public object GetSingle(string SQLString)
        {
            return DbHelperOleDb.GetSingle(SQLString);
        }


        public void GetColumns(string table, ref List<string> fields, ref string primaryKey)
        {
            DbHelperOleDb.GetColumns(table, ref fields, ref primaryKey);
        }

        private OleDbParameter[] ConvertToMySqlParameter(params CustomParameter[] parameters)
        {
            List<OleDbParameter> list = new List<OleDbParameter>();
            foreach (CustomParameter p in parameters)
            {
                list.Add(new OleDbParameter(p.Name, p.Value));
            }
            return list.ToArray();
        }




    }
}
