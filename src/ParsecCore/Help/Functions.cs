using System;
using System.Collections.Generic;
using System.Text;

namespace ParsecCore.Help
{
    internal static class Functions
    {
        /// <summary>
        /// Convert a list to pretty string with all of its members listed
        /// </summary>
        /// <typeparam name="T"> The type of list </typeparam>
        /// <param name="collection"> The list to convert </param>
        /// <param name="sep"> The separator to use, by default ',' </param>
        /// <returns> Pretty representation of the list </returns>
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

        /// <summary>
        /// Prepend a list with a value
        /// </summary>
        /// <typeparam name="T"> The type of list </typeparam>
        /// <param name="rest"> The list to prepend </param>
        /// <param name="value"> The value to add to the beginning of the list </param>
        /// <returns> New list with the value as its first member and rest of the list copied </returns>
        public static IReadOnlyList<T> Prepend<T>(this IReadOnlyList<T> rest, T value)
        {
            List<T> newList = new List<T>(rest.Count + 1);
            newList.Add(value);
            newList.AddRange(rest);

            return newList;
        }

        /// <summary>
        /// Append <see cref="IReadOnlyList{T}"/> with a value
        /// </summary>
        /// <typeparam name="T"> Type stored inside list </typeparam>
        /// <param name="prefix"> The list to append to </param>
        /// <param name="value"> The value to append </param>
        /// <returns> New list with an added value to the end </returns>
        public static IReadOnlyList<T> Append<T>(this IReadOnlyList<T> prefix, T value)
        {
            List<T> newList = new List<T>(prefix.Count + 1);
            newList.AddRange(prefix);
            newList.Add(value);

            return newList;
        }

        /// <summary>
        /// Map all values in <paramref name="values"/> and return the mapped values in a new list.
        /// </summary>
        /// <typeparam name="T"> The original type of the values </typeparam>
        /// <typeparam name="R"> The type of the mapped values </typeparam>
        /// <param name="values"> The values we are mapping </param>
        /// <param name="map"> The mapping function </param>
        /// <returns> A new list which contains the mapped values </returns>
        public static IReadOnlyList<R> Map<T, R>(this IReadOnlyList<T> values, Func<T, R> map)
        {
            List<R> mapped = new List<R>(values.Count);
            foreach (var item in values)
            {
                mapped.Add(map(item));
            }
            return mapped;
        }

        /// <summary>
        /// Concatenate two lists
        /// </summary>
        /// <typeparam name="T"> The type of lists </typeparam>
        /// <param name="left"> The list which will be at the front after the concatenation </param>
        /// <param name="right"> The list which will be at the end after the concatenation </param>
        /// <returns> The concatenated list </returns>
        public static List<T> Concat<T>(this List<T> left, List<T> right)
        {
            List<T> concat = new(left.Count + right.Count);
            concat.AddRange(left);
            concat.AddRange(right);

            return concat;
        }

        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            return new[] { value };
        }
    }
}
