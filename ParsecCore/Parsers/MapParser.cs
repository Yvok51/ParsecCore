using ParsecCore.EitherNS;
using System;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Class used in the Select extension method to enable "do" notation 
    /// (`from ... in ...`) for a single variable
    /// Performs the equaivalent to the fmap function
    /// </summary>
    /// <typeparam name="TSource"> The result type of the provided parser </typeparam>
    /// <typeparam name="TResult"> The result type of the parser with the result value mapped by the given function </typeparam>
    class MapParser
    {
        public static Parser<TResult, TInputToken> Parser<TSource, TResult, TInputToken>(
            Parser<TSource, TInputToken> sourceParser, Func<TSource, TResult> projection
        )
        {
            return (input) =>
            {
                return from value in sourceParser(input)
                       select projection(value);
            };
        }
    }
}
