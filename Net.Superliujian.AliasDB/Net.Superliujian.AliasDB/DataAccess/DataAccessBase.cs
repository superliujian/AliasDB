using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Maticsoft.DBUtility;

namespace Net.Superliujian.AliasDB
{
    /// <summary>
    /// 自适应的MySQL数据实体类
    /// 可以忽略数据库的改变而不用改变数据库访问接口
    /// 需要获取描述表信息
    ///名字有空格的时候：
    ///ODBC:""
    ///SQL Server：可用""或者[]
    ///MySQL:``
    ///SQLite:``或者[]或者""或者''
    /// </summary>
    public abstract class DataAccessBase
    {
        //数据表名：由数据表前缀+传入的表名
        private string _TableName = "";
        public string TableName { get { return _TableName; } protected set { _TableName = value; } }
        //列名
        private List<string> _Fields;
        public List<string> Fields { get { return _Fields; } set { _Fields = value; } }
        //主键名：暂时仅支持单一主键
        private string _PrimaryKey;
        public string PrimaryKey { get { return _PrimaryKey; } set { _PrimaryKey = value; } }
        //Parameter的分隔符，MySQL里为?,SQL Server等里为@
        private string _ParameterSeperator = "@";
        public string ParameterSeperator { get { return _ParameterSeperator; } set { _ParameterSeperator = value; } }
        protected IDbHelperWrapper _DbHelperWrapper = null;
        protected IDbHelperWrapper DbHelperWrapper
        {
            get { return _DbHelperWrapper; }
            set { _DbHelperWrapper = value; }
        }

        public string TableNameQuoted
        {
            get
            {
                if (TableName.Contains(" "))
                    return string.Format("{0}{1}{2}", NameQuotedLeft, TableName, NameQuotedRight);
                else return TableName;
            }
        }

        public virtual string NameQuotedLeft
        {
            get { return "\""; }
        }
        public virtual string NameQuotedRight
        {
            get { return "\""; }
        }

        /// <summary>
        /// 将一个以逗号为分隔的列名，重新组合成每列用引用符引用的字符串，以避免空格带来的SQL语句的不正确
        ///   在MSSQL中：a a,bb => "a a","bb"
        ///   在MySQL中：a a,bb => `a a`,`bb`
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string QuatedString(string source)
        {
            string[] tokens = source.Split(new char[] { ',' });
            StringBuilder sb = new StringBuilder();
            foreach (string token in tokens)
            {
                sb.AppendFormat("{0}{1}{2},", NameQuotedLeft, token, NameQuotedRight);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table">不包含表前缀的表名</param>
        public DataAccessBase(string table)
        {
            TableName = PubConstant.TablePrefix + table;// string.Format("\"{0}{1}\"", PubConstant.TablePrefix, table);
            InitializeDataAdaptor();
            //InitializeDataFields有可能会用到DbAdaptor，所以要先初始化InitializeDataAdaptor
            InitializeDataFields();
        }

        /// <summary>
        /// 获取表信息：列名和主键名
        /// </summary>
        protected virtual void InitializeDataFields()
        {
            _Fields = new List<string>();
            DbHelperWrapper.GetColumns(TableNameQuoted, ref _Fields, ref _PrimaryKey);
        }

        protected abstract void InitializeDataAdaptor();


        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(object id)
        {
            string sql = string.Format("select count(1) from {0} where {1}={2}{1}", TableNameQuoted, PrimaryKey, ParameterSeperator);
            return DbHelperWrapper.Exists(sql, new CustomParameter(ParameterSeperator + PrimaryKey, id));
        }

        /// <summary>
        /// 插入一行
        /// </summary>
        /// <param name="model">数据实体</param>
        /// <returns>是否插入成功</returns>
        public bool Insert(Model model)
        {
            StringBuilder strSql = new StringBuilder();
            string[] keys = new string[model.Keys.Count];
            model.Keys.CopyTo(keys, 0);
            strSql.Append("insert into " + TableNameQuoted + "(");
            strSql.Append(string.Join(",", keys));
            strSql.Append(") values (" + ParameterSeperator);
            strSql.Append(string.Join("," + ParameterSeperator, keys));
            strSql.Append(")");
            List<CustomParameter> parameters = new List<CustomParameter>();
            foreach (string key in keys)
            {
                parameters.Add(new CustomParameter(ParameterSeperator + key, model[key]));
            }
            int rows = DbHelperWrapper.ExecuteSql(strSql.ToString(), parameters.ToArray());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新一列
        /// </summary>
        /// <param name="model">数据实体</param>
        /// <returns>是否更新成功</returns>
        public bool Update(Model model)
        {
            StringBuilder strSql = new StringBuilder();
            string[] keys = new string[model.Keys.Count];
            model.Keys.CopyTo(keys, 0);

            strSql.Append("update " + TableNameQuoted + " set ");
            foreach (string key in keys)
            {
                if (key == PrimaryKey)
                    continue;
                strSql.Append(string.Format(" {0}={1}{0},", key, ParameterSeperator));
            }
            strSql.Remove(strSql.Length - 1, 1); //remove last ,
            strSql.Append(string.Format(" where {0}={1}{0}", PrimaryKey, ParameterSeperator));

            List<CustomParameter> parameters = new List<CustomParameter>();
            foreach (string key in keys)
            {
                parameters.Add(new CustomParameter(ParameterSeperator + key, model[key]));
            }

            int rows = DbHelperWrapper.ExecuteSql(strSql.ToString(), parameters.ToArray());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 通过主键判断是否存在，并智能地选择插入或更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertOrUpdate(Model model)
        {
            if (Exists(model[PrimaryKey]))
                return Update(model);
            else
                return Insert(model);
        }


        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(object id)
        {
            string sql = string.Format("delete from {0} where {1}={2}{1}", TableNameQuoted, PrimaryKey, ParameterSeperator);
            int rows = DbHelperWrapper.ExecuteSql(sql, new CustomParameter(ParameterSeperator + PrimaryKey, id));
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 通过数据实体删除一列
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Delete(Model model)
        {
            return Delete(model[PrimaryKey]);
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string idlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from " + TableNameQuoted);
            strSql.Append(" where " + PrimaryKey + " in (" + idlist + ")  ");
            int rows = DbHelperWrapper.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(params string[] idlist)
        {
            return DeleteList(string.Join(",", idlist));
        }


        /// <summary>
        /// 通过指定列和ID获取数据实体
        /// </summary>
        /// <param name="fields">以逗号分开的数据列，也可以是*</param>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public Model GetModel(string fields, object id)
        {
            Model model = new Model();
            string sql = string.Format("select {0} from {1} where {2}={3}{2}", fields, TableNameQuoted, PrimaryKey, ParameterSeperator);
            DataSet ds = DbHelperWrapper.Query(sql, new CustomParameter(ParameterSeperator + PrimaryKey, id));
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            List<Model> models = DataSet2ModelList(ds);
            return models[0];
        }


        /// <summary>
        /// 通过指定列和ID获取数据实体
        /// </summary>
        /// <param name="fields">以逗号分开的数据列，也可以是*</param>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        public Model GetModel(string fields, string where)
        {
            Model model = new Model();
            string sql = string.Format("select {0} from {1} where {2}", fields, TableNameQuoted, where);
            DataSet ds = DbHelperWrapper.Query(sql);
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            List<Model> models = DataSet2ModelList(ds);
            return models[0];
        }

        public Model GetModel(object id)
        {
            return GetModel("*", id);
        }

        /// <summary>
        /// 得到一串数据
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<Model> GetList(string fields, string where)
        {
            return DataSet2ModelList(GetListDataSet(fields, where));
        }

        /// <summary>
        /// 得到所有数据
        /// </summary>
        /// <returns></returns>
        public List<Model> GetAllList()
        {
            return GetList("*", "");
        }


        /// <summary>
        /// 得到一串数据
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public DataSet GetListDataSet(string fields, string where)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("select {0} from {1}", fields, TableNameQuoted));
            if (where.Trim() != "")
            {
                strSql.Append(" where " + where);
            }

            DataSet ds = DbHelperWrapper.Query(strSql.ToString());
            return ds;
        }

        /// <summary>
        /// 得到所有数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllListDataSet()
        {
            return GetListDataSet("*", "");
        }


        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int Total(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM " + TableNameQuoted);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            object obj = DbHelperWrapper.GetSingle(strSql.ToString());

            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        public int Total()
        {
            return Total("");
        }


        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public List<Model> GetListByPage(string strWhere, string orderby, int startIndex, int pageSize)
        {
            return DataSet2ModelList(GetListByPageDataSet(strWhere, orderby, startIndex, pageSize));
        }


        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPageDataSet(string strWhere, string orderby, int startIndex, int pageSize)
        {

            //SELECT * FROM `abc` WHERE `BatchID` = 123 ORDER BY InputDate DESC LIMIT 428775, 40  
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM " + TableNameQuoted);
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }

            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append(" order by " + orderby);
            }
            else
            {
                strSql.Append(" order by " + PrimaryKey + " desc");
            }

            strSql.AppendFormat(" LIMIT {0} , {1}", startIndex, pageSize);
            DataSet ds = DbHelperWrapper.Query(strSql.ToString());
            return ds;
        }


        /// <summary>
        /// DataSet to Model Lit
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private List<Model> DataSet2ModelList(DataSet ds)
        {
            List<Model> models = new List<Model>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Model model = new Model();
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    model[dc.ColumnName] = dr[dc].ToString();
                }
                models.Add(model);
            }
            return models;
        }


    }
}
