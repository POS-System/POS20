using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS20.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SqlAttribute : Attribute
    {
        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public SqlAttribute()
        {
            _name = "";            
        }
    }      

    public class SqlTableAttribute : SqlAttribute
    {     
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SqlJoinAttribute : SqlAttribute
    {
        private String _tableName;
        private String _fieldSource;
        private String _fieldTarget;
        private String _joinType;        

        public SqlJoinAttribute()
        {           
        }

        public string TableName
        {
            get
            {
                return _tableName;
            }

            set
            {
                _tableName = value;
            }
        }

        public string FieldSource
        {
            get
            {
                return _fieldSource;
            }

            set
            {
                _fieldSource = value;
            }
        }

        public string FieldTarget
        {
            get
            {
                return _fieldTarget;
            }

            set
            {
                _fieldTarget = value;
            }
        }

        public string JoinType
        {
            get
            {
                return _joinType;
            }

            set
            {
                _joinType = value;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SqlOutputColumnAttribute : SqlAttribute
    {
        private Type? _type;        
        private Int32 _size;
        private Int32 _parentID;

        public Type? Type
        {
            get { return _type; }
            set { _type = value; }
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
    public class SqlInputJoinColumnAttribute : Attribute
    {
        private Type? _type;
        private string _name;
        private string _tableName;
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

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
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

        public SqlInputJoinColumnAttribute()
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
