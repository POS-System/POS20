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
    public class Role : BaseObject, ISelect<Role>, ISelectSummary<Role>
    {
        string name;
        string description;

        public Role()
        {            
        }    

        [SqlOutputColumn()]
        [SqlInputColumn()]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        [SqlOutputColumn()]
        [SqlInputColumn()]
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }

        public List<Role> Select(BaseObject baseObject)
        {
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended(true);
            return sqlConnectionExtended.Select<Role>(this);
        }        

        public Int32 SelectSummary(BaseObject baseObject)
        {
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended(true);
            return sqlConnectionExtended.SelectSummary<Role>(this);
        }        
    }
}
