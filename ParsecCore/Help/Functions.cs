using System.Collections.Generic;
using System.Text;

namespace ParsecCore.Help
{
    static class Functions
    {
        public static string ToPrettyString<T>(this IEnumerable<T> collection, string sep = ",")
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
    }
}
