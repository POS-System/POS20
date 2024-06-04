using POS20.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS20.Interface
{
    public interface ISave
    {
        public Int32 Save();
    }

    public interface ISelect<T>
    {
        public List<T> Select(BaseObject baseObject);
    }
}
