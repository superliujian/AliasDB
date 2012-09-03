using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Net.Superliujian.AliasDB
{
    public interface IDbHelperWrapper
    {
        int ExecuteSql(string sql, params CustomParameter[] parameters);
        DataSet Query(string sql, params CustomParameter[] parameters);
        bool Exists(string sql, params CustomParameter[] parameters);
        object GetSingle(string SQLString);
        void GetColumns(string table, ref List<string> fields, ref string primaryKey);

    }
}
