using System;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Returns a parser which parses a value in between two other values
        /// Usefull for quoted strings, arrays, ...
        /// </summary>
        /// <typeparam name="TLeft"> The result type of the parser of the left value </typeparam>
        /// <typeparam name="TBetween"> The result type of the parser of the inbetween value </typeparam>
        /// <typeparam name="TRight"> The result type of the parser of the left value </typeparam>
        /// <param name="leftParser"> The parser for the value on the left </param>
        /// <param name="betweenParser"> The parser for the value inbetween </param>
        /// <param name="rightParser"> The parser for the value on the right </param>
        /// <returns> 
        /// Parser which parses the entire sequence of
        /// <paramref name="leftParser"/>-<paramref name="betweenParser"/>-<paramref name="rightParser"/>
        /// but only returns the value parsed by the <paramref name="betweenParser"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<TBetween, TInputToken> Between<TLeft, TBetween, TRight, TInputToken>(
            Parser<TLeft, TInputToken> leftParser,
            Parser<TBetween, TInputToken> betweenParser,
            Parser<TRight, TInputToken> rightParser
        )
        {
            if (leftParser is null) throw new ArgumentNullException(nameof(leftParser));
            if (betweenParser is null) throw new ArgumentNullException(nameof(betweenParser));
            if (rightParser is null) throw new ArgumentNullException(nameof(rightParser));

            return leftParser.Then(betweenParser.FollowedBy(rightParser));
        }

        /// <summary>
        /// Returns a parser which parses a value in between two other values
        /// Usefull for quoted strings, arrays, ...
        /// </summary>
        /// <typeparam name="TOutside"> The result type of the parser of the outside values </typeparam>
        /// <typeparam name="TBetween"> The result type of the parser of the inbetween value </typeparam>
        /// <param name="outsideParser"> The parser for the outside values </param>
        /// <param name="betweenParser"> The parser for the value inbetween </param>
        /// <returns>
        /// <paramref name="outsideParser"/>-<paramref name="betweenParser"/>-<paramref name="outsideParser"/>
        /// but only returns the value parsed by the <paramref name="betweenParser"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<TBetween, TInputToken> Between<TOutside, TBetween, TInputToken>(
            Parser<TOutside, TInputToken> outsideParser,
            Parser<TBetween, TInputToken> betweenParser
        ) =>
            Between(outsideParser, betweenParser, outsideParser);

    }
}
