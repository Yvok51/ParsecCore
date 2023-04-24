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
        public static List<T> Prepend<T>(this IReadOnlyList<T> rest, T value)
        {
            List<T> newList = new List<T>(rest.Count + 1);
            newList.Add(value);
            newList.AddRange(rest);

            return newList;
        }

        /// <summary>
        /// Prepend a list with a value
        /// </summary>
        /// <typeparam name="T"> The type of list </typeparam>
        /// <param name="rest"> The list to prepend </param>
        /// <param name="value"> The value to add to the beginning of the list </param>
        /// <returns> The same list with <paramref name="value"/> prepended to the beginning of the list </returns>
        public static List<T> Prepend<T>(this List<T> rest, T value)
        {
            rest.Insert(0, value);
            return rest;
        }

        /// <summary>
        /// Append <see cref="IReadOnlyList{T}"/> with a value
        /// </summary>
        /// <typeparam name="T"> Type stored inside list </typeparam>
        /// <param name="prefix"> The list to append to </param>
        /// <param name="value"> The value to append </param>
        /// <returns> New list with an added value to the end </returns>
        public static List<T> Append<T>(this IReadOnlyList<T> prefix, T value)
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
        public static List<R> Map<T, R>(this IReadOnlyList<T> values, Func<T, R> map)
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

        /// <summary>
        /// Aggregate items in a list from right.
        /// </summary>
        /// <typeparam name="T"> The type in the list </typeparam>
        /// <param name="list"> The list of items to aggreagate </param>
        /// <param name="func"> The function to aggregate with </param>
        /// <returns>
        /// The aggregate of the <paramref name="list"/> items made by the function <paramref name="func"/>
        /// </returns>
        /// <exception cref="InvalidOperationException"> If the list is empty </exception>
        public static T RightAggregate<T>(this IReadOnlyList<T> list, Func<T, T, T> func)
        {
            T accum = list[list.Count - 1];
            for (int i = list.Count - 2; i >= 0; i--)
            {
                accum = func(accum, list[i]);
            }
            return accum;
        }

        /// <summary>
        /// Aggregate items in a list from right.
        /// </summary>
        /// <typeparam name="T"> The type in the list </typeparam>
        /// <typeparam name="R"> The return type </typeparam>
        /// <param name="list"> The list of items to aggreagate </param>
        /// <param name="func"> The function to aggregate with </param>
        /// <param name="start"> The item to start the aggregation with </param>
        /// <returns>
        /// The aggregate of the <paramref name="list"/> items made by the function <paramref name="func"/>
        /// </returns>
        /// <exception cref="InvalidOperationException"> If the list is empty </exception>
        public static R RightAggregate<T, R>(this IReadOnlyList<T> list, Func<R, T, R> func, R start)
        {
            R accum = start;
            foreach (var item in list)
            {
                accum = func(accum, item);
            }
            return accum;
        }
    }
}
