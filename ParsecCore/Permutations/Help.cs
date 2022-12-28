using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore.Permutations
{
    static class Help
    {
        internal static string Join<T>(IEnumerable<T> collection, Func<T, string> map, string seperator)
        {
            var mapped = collection.Select(map);
            return string.Join(seperator, mapped);
        }
    }
}
