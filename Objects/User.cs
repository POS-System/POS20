using GIIS21.SqlEngine;
using POS20.Attributes;
using POS20.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS20.Objects
{
    [SqlTable()]    
    public class User : BaseObject, ISelect<User>, ISelectSummary<User>
    {
        private String firstName;
        private String lastName;
        private String secondName;

        private Int32 roleID;
        private String inn;


        public User()
        {            
        }

        [SqlInputColumn()]        
        public override int ID
        {
            get
            {
                return base.ID;
            }

            set
            {
                base.ID = value;
            }
        }

        [SqlOutputColumn()]
        [SqlInputColumn()]
        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                firstName = value;
            }
        }

        [SqlOutputColumn()]
        [SqlInputColumn()]
        public string LastName
        {
            get
            {
                return lastName;
            }

            set
            {
                lastName = value;
            }
        }

        [SqlOutputColumn()]
        [SqlInputColumn()]
        public string SecondName
        {
            get
            {
                return secondName;
            }

            set
            {
                secondName = value;
            }
        }

        [SqlOutputColumn()]
        [SqlInputColumn()]
        public int RoleID
        {
            get
            {
                return roleID;
            }

            set
            {
                roleID = value;
            }
        }

        [SqlOutputColumn()]
        [SqlInputColumn()]
        public string Inn
        {
            get
            {
                return inn;
            }

            set
            {
                inn = value;
            }
        }

        public List<User> Select(BaseObject baseObject)
        {
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended(true);
            return sqlConnectionExtended.Select<User>(this);
        }        

        public Int32 SelectSummary(BaseObject baseObject)
        {
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended(true);
            return sqlConnectionExtended.SelectSummary<User>(this);
        }        
    }

    [SqlTable(Name = "User")]
    public class UserFilter : BaseFilter, ICollectFilter
    {        
        public UserFilter()
        {
        }

        Int32 iDMin;
        Int32 iDMax;

        DataTable roleID; 

        [Filter(Type = FilterType.Int32, IsMin = true, Name = "ID")]
        public int IDMin
        {
            get
            {
                return iDMin;
            }

            set
            {
                iDMin = value;
            }
        }

        [Filter(Type = FilterType.Int32, IsMax = true, Name = "ID")]
        public int IDMax
        {
            get
            {
                return iDMax;
            }

            set
            {
                iDMax = value;
            }
        }

        [Filter(Type = FilterType.Table, Name = "RoleID", TableName = "Role", Value="ID", Description = "Description" )]
        public DataTable RoleID
        {
            get
            {
                return roleID;
            }

            set
            {
                roleID = value;
            }
        }

        public void CollectFilter()
        {
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended(true);
            sqlConnectionExtended.CollectFilter(this);
        }
    }
}
