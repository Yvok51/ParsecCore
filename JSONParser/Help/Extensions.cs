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
                if (left[i] is null && right[i] is null)
                {
                    continue;
                }
                if (left[i] is null || right[i] is null || !left[i].Equals(right[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool MemberwiseEquals<TKey, TValue>(this Dictionary<TKey, TValue> left, Dictionary<TKey, TValue> right)
        {
            if (left is null || right is null || left.Count != right.Count)
            {
                return false;
            }

            foreach (var (key, value) in left)
            {
                bool present = right.TryGetValue(key, out TValue rightValue);
                if (!present)
                {
                    return false;
                }
                if (value is null && rightValue is null)
                {
                    continue;
                }
                if (value is null || rightValue is null || !value.Equals(rightValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
