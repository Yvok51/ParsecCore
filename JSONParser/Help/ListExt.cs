using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONtoXML
{
    static class Help
    {
        public static List<T> ListOf<T>(params T[] ts) => new List<T>(ts);
    }
}
