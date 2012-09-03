using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Superliujian.AliasDB
{
    /// <summary>
    /// 数据库实体类
    /// 以字典形式表现
    /// 注意的是：列名不区分大小写
    /// </summary>
    public class Model : Dictionary<string, object>
    {
        public Model() : base() { }

        //重写this索引，使之对key的大小写不敏感
        public new object this[string key]
        {
            get { return base[key.ToLower()]; }
            set { base[key.ToLower()] = value; }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in Keys)
            {
                sb.Append(string.Format("{0}:{1};", key, this[key]));
            }
            return sb.ToString();
        }
    }
}
