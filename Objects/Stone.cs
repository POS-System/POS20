using POS20.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS20.Objects
{
    [SqlTable(Name = "Stone")]
    public class Stone : ChildBaseObject
    {
        private Int32 type;        

        [SqlOutputColumn(Name = "Type")]
        public Int32 Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }        
    }
}
