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

        public static bool MemberwiseEquals<T>(this List<T> left, List<T> right)
        {
            if (left is null || right is null || left.Count != right.Count)
            {
                return false;
            }

            for (int i = 0; i < left.Count; i++)
            {
                if (left is null && right is null)
                {
                    continue;
                }
                if (left is null || right is null || !left[i].Equals(right[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
