using System.Collections.Generic;
using System.Text;

namespace ParsecCore.Help
{
    static class Functions
    {
        public static string ToPrettyString<T>(this IReadOnlyList<T> collection, string sep = ",")
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            foreach (var item in collection)
            {
                builder.Append(" ");
                builder.Append(item);
                builder.Append(sep);
            }
            builder.Remove(builder.Length - sep.Length, sep.Length);
            builder.Append("]");

            return builder.ToString();
        }

        public static IReadOnlyList<T> Prepend<T>(this IReadOnlyList<T> rest, T value)
        {
            List<T> newList = new List<T>(rest.Count + 1);
            newList.Add(value);
            newList.AddRange(rest);

            return newList;
        }

        public static List<T> Concat<T>(this List<T> left, List<T> right)
        {
            List<T> concat = new(left.Count + right.Count);
            concat.AddRange(left);
            concat.AddRange(right);

            return concat;
        }
    }
}
