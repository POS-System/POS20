using GIIS21.SqlEngine;
using POS20.Attributes;
using POS20.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS20.Objects
{
    [SqlTable(Name = "Product")]
    public class Product : BaseObject, ISelect<Product>
    {
        private List<Stone> stones;
        private String model;

        public Product()
        {
            stones = new List<Stone>();
        }        

        [SqlOutputColumn(Name = "Model", Type = typeof(String), Size = 40)]
        [SqlInputColumn(Name = "Model", Type = typeof(String), Size = 40)]        
        public string Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;
            }
        }

        [ChildAttribute()]
        public List<Stone> Stones
        {
            get
            {
                return stones;
            }

            set
            {
                stones = value;
            }
        }

        public List<Product> Select(BaseObject baseObject)
        {
            SqlConnectionExtended sqlConnectionExtended = new SqlConnectionExtended(true);
            return sqlConnectionExtended.Select<Product>(this);
        }
    }
}
