using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Maticsoft.DBUtility;
using System.Data.OracleClient;

namespace Net.Superliujian.AliasDB
{
    public class OracleDbHelperWrapper : IDbHelperWrapper
    {
        public int ExecuteSql(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOracle.ExecuteSql(sql, ConvertToMySqlParameter(parameters));
        }

        public System.Data.DataSet Query(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOracle.Query(sql, ConvertToMySqlParameter(parameters));
        }


        public bool Exists(string sql, params CustomParameter[] parameters)
        {
            return DbHelperOracle.Exists(sql, ConvertToMySqlParameter(parameters));
        }

        public object GetSingle(string SQLString)
        {
            return DbHelperOracle.GetSingle(SQLString);
        }

        public void GetColumns(string table, ref List<string> fields, ref string primaryKey)
        {
            DbHelperOracle.GetColumns(table, ref fields, ref primaryKey);
        }

        private OracleParameter[] ConvertToMySqlParameter(params CustomParameter[] parameters)
        {
            List<OracleParameter> list = new List<OracleParameter>();
            foreach (CustomParameter p in parameters)
            {
                list.Add(new OracleParameter(p.Name, p.Value));
            }
            return list.ToArray();
        }


    }
}
