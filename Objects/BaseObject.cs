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
        
        public virtual long TimeStamp
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
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended();
            return sqlConnectionExtended.Save(this);
        }
    }

    public class ChildBaseObject : BaseObject
    {
        Int32 _parentID;

        [SqlColumn(Name = "ParentID")]
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
