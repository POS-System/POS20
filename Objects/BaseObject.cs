using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GIIS21.SqlEngine;
using POS20.Attributes;
using POS20.Interface;
using POS20.SqlHelper;

namespace POS20.Objects
{
    public class BaseObject : ISave
    {
        private Int32 _iD;        
        private Int64 _timeStamp;

        [SqlInputColumn()]
        public virtual int ID
        {
            get
            {
                return _iD;
            }

            set
            {
                _iD = value;
            }
        }

        [SqlInputColumn(Type = typeof(byte[]))]
        public Int64 TimeStamp
        {
            get
            {
                return _timeStamp;
            }

            set
            {
                _timeStamp = value;
            }
        }

        public virtual int Save()
        {  
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended(true);
            return sqlConnectionExtended.Save(this);
        }

        /*public virtual List<T> Select(BaseObject baseObject)
        {
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended();
            return sqlConnectionExtended.Select<T>(this);
        }*/
    }

    public class ChildBaseObject : BaseObject
    {
        Int32 _parentID;

        [SqlOutputColumn(Name = "ParentID")]
        public virtual int ParentID
        {
            get
            {
                return _parentID;
            }

            set
            {
                _parentID = value;
            }
        }
    }
}
