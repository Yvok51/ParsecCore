using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParsecCore.Permutations
{
    static class Help
    {
        internal static string Join<T>(IEnumerable<T> collection, Func<T, string> map, string seperator)
        {
            var mapped = collection.Select(map);
            return string.Join(seperator, mapped);
        }

        /// <summary>
        /// Generates a list of type names
        /// </summary>
        /// <param name="numberOfTypes"> Number of types to generate </param>
        /// <returns> List of type names </returns>
        public static IReadOnlyList<string> GetTypesList(int numberOfTypes)
        {
            List<string> types = new(numberOfTypes);
            for (int i = 1; i <= numberOfTypes; i++)
            {
                types.Add($"T{i}");
            }
            return types;
        }

        public static IEnumerable<(T, IReadOnlyList<T>)> PickOneReturnRest<T>(this List<T> values) 
        {
            for (int i = 0; i < values.Count; i++)
            {
                var lhs = values.GetRange(0, i);
                var rhs = values.GetRange(i + 1, values.Count - i - 1);
                lhs.AddRange(rhs);
                yield return (values[i], lhs);
            }
        }
    }
}
