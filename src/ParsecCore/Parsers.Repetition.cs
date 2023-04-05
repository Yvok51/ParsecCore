using ParsecCore.Help;
using ParsecCore.ParsersHelp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Returns a parser which tries to parse according to the given parser as many times as possible.
        /// Can parse even zero times.
        /// If parsing fails in the middle of one iteration (some input was consumed),
        /// then the parser fails regardless of how many previous iterations were parsed already.
        /// If this behaviour is not desired add the modifier 
        /// <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> to the parser beforehand.
        /// </summary>
        /// <typeparam name="T"> The type of the parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> Parser which applies the given parser as many times as possible </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<IReadOnlyList<T>, TInputToken> Many<T, TInputToken>(
            this Parser<T, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return ManyParser.Parser(parser);
        }

        /// <summary>
        /// Returns a parser which tries to parse according to the given parser as many times as possible.
        /// Must be sucessful at least once otherwise an error is returned.
        /// If parsing fails in the middle of one iteration (some input was consumed),
        /// then the parser fails regardless of how many previous iterations were parsed already.
        /// If this behaviour is not desired add the modifier 
        /// <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> to the parser beforehand.
        /// </summary>
        /// <typeparam name="T"> The type of the parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> Parser which applies the given parser as many times as possible </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<IReadOnlyList<T>, TInputToken> Many1<T, TInputToken>(
            this Parser<T, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return from firstParse in parser
                   from restParses in parser.Many()
                   select restParses.Prepend(firstParse);
        }

        /// <summary>
        /// Specialization of the <see cref="Many{T, TInputToken}(Parser{T, TInputToken})"/>
        /// method for chars and strings.
        /// </summary>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to apply as many times as possible </param>
        /// <returns> Parser which applies the given char parser as many times as possible </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<string, TInputToken> Many<TInputToken>(
            this Parser<char, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return from chars in parser.Many<char, TInputToken>()  // added explicit char to avoid recursion
                   select string.Concat(chars);
        }

        /// <summary>
        /// Specialization of the <see cref="Many1{T, TInputToken}(Parser{T, TInputToken})"/> method for chars and strings
        /// </summary>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to apply as many times as possible </param>
        /// <returns> Parser which applies the given char parser as many times as possible </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<string, TInputToken> Many1<TInputToken>(
            this Parser<char, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return from firstParse in parser
                   from restParses in parser.Many()
                   select firstParse.ToString() + restParses;
        }

        /// <summary>
        /// Aplies the parser as many times as possible and ignores the result.
        /// <see cref="Many{T, TInputToken}(Parser{T, TInputToken})"/> for specifics.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> 
        /// Parser which applies the given parser as many times as possible and then ignores its result.
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<None, TInputToken> SkipMany<T, TInputToken>(
            this Parser<T, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return parser.Many().Void();
        }

        /// <summary>
        /// Applies the parser as many times as possible (but at least once) and ignores the result.
        /// See <see cref="Many1{T, TInputToken}(Parser{T, TInputToken})"/>.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <returns> 
        /// Parser which applies the given parser as many times as possible and then ignores its result.
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<None, TInputToken> SkipMany1<T, TInputToken>(
            this Parser<T, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return parser.Many1().Void();
        }

        /// <summary>
        /// Aplies the parser a given amount of times.
        /// If the count is zero or less, then the returned parser is equivalent to
        /// the <see cref="Parsers.Return{T}(T)">ReturnParser</see> of an empty enumerable.
        /// If the aplication of <paramref name="parser"/> <paramref name="count"/> number of times fails,
        /// then the entire parser fails and consumes any input already consumed.
        /// </summary>
        /// <typeparam name="T"> The type of parser to apply </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to apply </param>
        /// <param name="count"> Number of times to apply the parser </param>
        /// <returns> Parser which applies itself the given number of times </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<IReadOnlyList<T>, TInputToken> Count<T, TInputToken>(
            this Parser<T, TInputToken> parser,
            int count
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return CountParser.Parser(parser, count);
        }

        /// <summary>
        /// Applies <paramref name="parser"/> zero or more times until parser <paramref name="till"/> succeeds.
        /// Returns the list of values returned by <paramref name="parser"/>.
        /// Consumes 
        /// <para>
        /// If the <paramref name="till"/> is never encountered (until the end of file) then the parser fails.
        /// If the <paramref name="parser"/> fails before <paramref name="till"/> is encountered,
        /// then the entire parser fails.
        /// If the <paramref name="till"/> fails while consuming input, then the entire parser fails,
        /// use <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> to change this behaviour
        /// </para>
        /// <para>
        /// Consumes all input consumed by both <paramref name="parser"/> and <paramref name="till"/> parsers.
        /// If you want to change this behaviour use <see cref="LookAhead{T, TInputToken}(Parser{T, TInputToken})"/>,
        /// especially on <paramref name="till"/> parser.
        /// </para>
        /// </summary>
        /// <typeparam name="T"> The return type of the value parser </typeparam>
        /// <typeparam name="TEnd"> The return type of the till parser </typeparam>
        /// <param name="parser"> The value parser </param>
        /// <param name="till"> The end parser </param>
        /// <returns> Parser which applies <c>parser</c> untill <c>till</c> succeeds </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<IReadOnlyList<T>, TInputToken> ManyTill<T, TEnd, TInputToken>(
            Parser<T, TInputToken> parser,
            Parser<TEnd, TInputToken> till
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (till is null) throw new ArgumentNullException(nameof(till));

            return ManyTillParser.Parser(parser, till);
        }
    }
}
