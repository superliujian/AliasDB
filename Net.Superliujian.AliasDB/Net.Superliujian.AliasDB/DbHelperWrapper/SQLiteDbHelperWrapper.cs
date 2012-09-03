using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using Maticsoft.DBUtility;

namespace Net.Superliujian.AliasDB
{
    public class SQLiteDbHelperWrapper : IDbHelperWrapper
    {
        public int ExecuteSql(string sql, params CustomParameter[] parameters)
        {
            return DbHelperSQLite.ExecuteSql(sql, ConvertToSQLiteParameter(parameters));
        }

        public System.Data.DataSet Query(string sql, params CustomParameter[] parameters)
        {
            return DbHelperSQLite.Query(sql, ConvertToSQLiteParameter(parameters));
        }


        public bool Exists(string sql, params CustomParameter[] parameters)
        {
            return DbHelperSQLite.Exists(sql, ConvertToSQLiteParameter(parameters));
        }


        public object GetSingle(string SQLString)
        {
            return DbHelperSQLite.GetSingle(SQLString);
        }

        public void GetColumns(string table, ref List<string> fields, ref string primaryKey)
        {
            DbHelperSQLite.GetColumns(table, ref fields, ref primaryKey);
        }

        private SQLiteParameter[] ConvertToSQLiteParameter(params CustomParameter[] parameters)
        {
            List<SQLiteParameter> list = new List<SQLiteParameter>();
            foreach (CustomParameter p in parameters)
            {
                list.Add(new SQLiteParameter(p.Name, p.Value));
            }
            return list.ToArray();
        }
    }
}
