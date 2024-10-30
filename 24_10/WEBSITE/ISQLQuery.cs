using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _23_10.Website
{
    public interface ISQLQuery
    {
        void Insert(Data data);

        bool findById(string id);
    }
}
