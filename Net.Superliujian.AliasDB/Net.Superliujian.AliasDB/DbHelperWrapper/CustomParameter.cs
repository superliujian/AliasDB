using System;
using System.Collections.Generic;
using System.Text;

namespace Net.Superliujian.AliasDB
{
    public class CustomParameter
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }
        private object _Value;
        public object Value { get { return _Value; } set { _Value = value; } }

        public CustomParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
