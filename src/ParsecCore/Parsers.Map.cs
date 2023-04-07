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

            return parser.MapConstant(None.Instance);
        }

        /// <summary>
        /// Map the result of the parser.
        /// Equivalent to 
        /// <see cref="Select{TSource, TResult, TInputToken}(Parser{TSource, TInputToken}, Func{TSource, TResult})"/>.
        /// If <paramref name="parser"/> fails, then returned parser also fails.
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

        /// <summary>
        /// Change the result of the parser to a constant value.
        /// Equivalent to <see cref="Map{T, TResult, TInput}(Parser{T, TInput}, Func{T, TResult})"/> with a constant
        /// function, but is faster.
        /// If <paramref name="parser"/> fails, then returned parser also fails.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <typeparam name="TResult"> The type of the constant </typeparam>
        /// <typeparam name="TInput"> The input type of the parser </typeparam>
        /// <param name="parser"> Parser to apply </param>
        /// <param name="value"> Value to return if parse was successful </param>
        /// <returns>
        /// Parser which parses the same as <paramref name="parser"/> but always returns the same constant
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<TResult, TInput> MapConstant<T, TResult, TInput>(
            this Parser<T, TInput> parser,
            TResult value
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (value is null) throw new ArgumentNullException(nameof(value));

            return (input) =>
            {
                var res = parser(input);
                if (res.IsResult)
                {
                    return Result.Success(value, res.UnconsumedInput);
                }
                else
                {
                    return Result.RetypeError<T, TResult, TInput>(res);
                }
            };
        }
    }
}
