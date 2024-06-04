using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS20.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SqlTableAttribute : Attribute
    {
        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public SqlTableAttribute()
        {
            _name = "";            
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SqlOutputColumnAttribute : Attribute
    {
        private Type? _type;
        private string _name;
        private Int32 _size;
        private Int32 _parentID;

        public Type? Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value;  }
        }

        public Int32 Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public Int32 ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }

        public SqlOutputColumnAttribute()
        {
            _type = null;
            _name = "";
            _size = 255;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SqlInputColumnAttribute : Attribute
    {
        private Type? _type;
        private string _name;
        private Int32 _size;
        private Int32 _parentID;

        public Type? Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Int32 Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public Int32 ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }

        public SqlInputColumnAttribute()
        {
            _type = null;
            _name = "";
            _size = 255;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ChildAttribute : Attribute
    {
    }
}
