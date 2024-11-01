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
        private byte[] _timeStamp;
        private DateTime createdDate;
        private DateTime lastModifiedDate;
        private Int32 createdByUserID;
        private Int32 lastModifiedByUserID;

        private String sort;
        private Boolean sortDirection;        

        private Int32 from;
        private Int32 to;
        private Int32 page;
        private Int32 rowsForPage;

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

        [SqlInputColumn(Name = "TimeStamp", Type = typeof(byte[]))]
        public Byte[] TimeStamp
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

        
        public String SqlTimeStamp
        {
            get
            {
                return $"0x{BitConverter.ToString(_timeStamp).Replace("-", "")}";
            }            
        }

        public String SqlDirection
        {
            get
            {
                return SortDirection == true ? "DESC" : "ASC";
            }
        }

        [SqlInputColumn()]
        [SqlOutputColumn()]
        public DateTime CreatedDate
        {
            get
            {
                return createdDate;
            }

            set
            {
                createdDate = value;
            }
        }

        [SqlInputColumn()]
        [SqlOutputColumn()]
        public DateTime LastModifiedDate
        {
            get
            {
                return lastModifiedDate;
            }

            set
            {
                lastModifiedDate = value;
            }
        }

        [SqlInputColumn()]
        [SqlOutputColumn()]
        public int CreatedByUserID
        {
            get
            {
                return createdByUserID;
            }

            set
            {
                createdByUserID = value;
            }
        }

        [SqlInputColumn()]
        [SqlOutputColumn()]
        public int LastModifiedByUserID
        {
            get
            {
                return lastModifiedByUserID;
            }

            set
            {
                lastModifiedByUserID = value;
            }
        }

        public int From
        {
            get
            {
                return Page*RowsForPage;
            }
        }

        public int To
        {
            get
            {
                return (Page+1)*(RowsForPage);
            }
        }

        public int Page
        {
            get
            {
                return page;
            }

            set
            {
                page = value;
            }
        }

        public int RowsForPage
        {
            get
            {
                return rowsForPage;
            }

            set
            {
                rowsForPage = value;
            }
        }

        public string Sort
        {
            get
            {
                return sort;
            }

            set
            {
                sort = value;
            }
        }

        public bool SortDirection
        {
            get
            {
                return sortDirection;
            }

            set
            {
                sortDirection = value;
            }
        }

        public virtual int Save()
        {  
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended(true);
            return sqlConnectionExtended.Save(this);
            // после любого сохранения необходимо перечитать таймстамп
        }

        public BaseObject()
        {
            Sort = "ID";
            SortDirection = false;
            Page = 0;
            RowsForPage = Int32.MaxValue;
        }
    }

    [SqlJoinAttribute(TableName = "User", FieldSource = "CreatedByUserID", FieldTarget = "ID", JoinType = "LEFT", Name = "u1")]
    [SqlJoinAttribute(TableName = "User", FieldSource = "LastModifiedByUserID", FieldTarget = "ID", JoinType = "LEFT", Name = "u2")]
    public class BaseObjectWithUser : BaseObject
    {
        private String createdByUserID_FirstName;
        private String createdByUserID_LastName;
        private String createdByUserID_SecondName;

        private String lastModifiedByUserID_FirstName;
        private String lastModifiedByUserID_LastName;
        private String lastModifiedByUserID_SecondName;

        private String createdByUserIDString;
        private String lastModifiedByUserIDString;

        [SqlInputJoinColumn(TableName = "u1", Name = "FirstName")]
        public string CreatedByUserID_FirstName
        {
            get
            {
                return createdByUserID_FirstName;
            }

            set
            {
                createdByUserID_FirstName = value;
            }
        }

        [SqlInputJoinColumn(TableName = "u1", Name = "LastName")]
        public string CreatedByUserID_LastName
        {
            get
            {
                return createdByUserID_LastName;
            }

            set
            {
                createdByUserID_LastName = value;
            }
        }

        [SqlInputJoinColumn(TableName = "u1", Name = "SecondName")]
        public string CreatedByUserID_SecondName
        {
            get
            {
                return createdByUserID_SecondName;
            }

            set
            {
                createdByUserID_SecondName = value;
            }
        }

        [SqlInputJoinColumn(TableName = "u2", Name = "FirstName")]
        public string LastModifiedByUserID_FirstName
        {
            get
            {
                return lastModifiedByUserID_FirstName;
            }

            set
            {
                lastModifiedByUserID_FirstName = value;
            }
        }

        [SqlInputJoinColumn(TableName = "u2", Name = "LastName")]
        public string LastModifiedByUserID_LastName
        {
            get
            {
                return lastModifiedByUserID_LastName;
            }

            set
            {
                lastModifiedByUserID_LastName = value;
            }
        }

        [SqlInputJoinColumn(TableName = "u2", Name = "SecondName")]
        public string LastModifiedByUserID_SecondName
        {
            get
            {
                return lastModifiedByUserID_SecondName;
            }

            set
            {
                lastModifiedByUserID_SecondName = value;
            }
        }

        public string CreatedByUserIDString
        {
            get
            {
                string LastName = "";
                string SecondName = "";
                string FirstName = "";

                LastName = CreatedByUserID_LastName;
                if (!String.IsNullOrEmpty(CreatedByUserID_FirstName))
                {
                    FirstName = $"{CreatedByUserID_FirstName[0]}.";
                }
                if (!String.IsNullOrEmpty(CreatedByUserID_SecondName))
                {
                    SecondName = $"{CreatedByUserID_SecondName[0]}.";
                }

                return $"{LastName} {FirstName} {SecondName}";
            }
        }

        public string LastModifiedByUserIDString
        {
            get
            {
                string LastName = "";
                string SecondName = "";
                string FirstName = "";

                LastName = LastModifiedByUserID_LastName;
                if (!String.IsNullOrEmpty(LastModifiedByUserID_FirstName))
                {
                    FirstName = $"{LastModifiedByUserID_FirstName[0]}.";
                }
                if (!String.IsNullOrEmpty(LastModifiedByUserID_SecondName))
                {
                    SecondName = $"{LastModifiedByUserID_SecondName[0]}.";
                }

                return $"{LastName} {FirstName} {SecondName}";
            }
        }        
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
