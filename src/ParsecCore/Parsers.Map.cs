using ParsecCore.Help;
using System;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Ignore the parsers return value and instead return <see cref="None"/>
        /// </summary>
        /// <typeparam name="T"> Type returned by the input parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> Parser whose return value is ignored </param>
        /// <returns> Parser whose return value is ignored </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<None, TInputToken> Void<T, TInputToken>(
            this Parser<T, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return from _ in parser
                   select None.Instance;
        }

        /// <summary>
        /// Map the result of the parser.
        /// Equivalent to 
        /// <see cref="Select{TSource, TResult, TInputToken}(Parser{TSource, TInputToken}, Func{TSource, TResult})"/>.
        /// </summary>
        /// <typeparam name="T"> Type of the parsing result </typeparam>
        /// <typeparam name="TResult"> Type returned by the new parser </typeparam>
        /// <typeparam name="TInput"> Type of the input token </typeparam>
        /// <param name="parser"> Parser whose result to map </param>
        /// <param name="map"> Mapping function for the result of the parsing </param>
        /// <returns> Parser with its result mapped according to <paramref name="map"/> </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<TResult, TInput> Map<T, TResult, TInput>(
            this Parser<T, TInput> parser,
            Func<T, TResult> map
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (map is null) throw new ArgumentNullException(nameof(map));

            return parser.Select(map);
        }
    }
}
