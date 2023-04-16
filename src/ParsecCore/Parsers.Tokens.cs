using ParsecCore.ParsersHelp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Returns a parser which parses exactly the string given
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string </returns>
        /// <exception cref="ArgumentNullException"> If provided string is null </exception>
        public static Parser<string, char> String(string stringToParse)
        {
            if (stringToParse is null) throw new ArgumentNullException(nameof(stringToParse));

            return AllParser.Parser(ToCharParsers(stringToParse)).MapConstant(stringToParse);
        }

        private static IReadOnlyList<Parser<char, char>> ToCharParsers(IEnumerable<char> chars)
            => chars.Select(c => Char(c)).ToArray();

        /// <summary>
        /// Returns a parser which ignores any whitespace after the parsed value.
        /// Uses the <see cref="Spaces"/> parser to parse subesquent whitespace.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <param name="parser"> What value to parse before the whitespace </param>
        /// <returns>
        /// Parser which parses the same value as the input parser followed by an arbitrary amount of whitespace.
        /// </returns>
        /// <exception cref="ArgumentNullException"> If parser is null </exception>
        public static Parser<T, char> Token<T>(Parser<T, char> parser)
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return parser.FollowedBy(Spaces);
        }

        /// <summary>
        /// Returns a parser which ignores any whitespace after the parsed value
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <typeparam name="TSpace"> The type returned by the whitespace parser </typeparam>
        /// <typeparam name="TInput"> The type of the input </typeparam>
        /// <param name="parser"> What value to parse between the whitespace </param>
        /// <param name="spaceConsumer"> Parser for the whitespace to consume </param>
        /// <returns>
        /// Parser which parses the same value as the input parser followed by whitespace.
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, TInput> Token<T, TSpace, TInput>(
            Parser<T, TInput> parser,
            Parser<TSpace, TInput> spaceConsumer
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));

            return parser.FollowedBy(spaceConsumer);
        }

        /// <summary>
        /// Returns a parser which parses exactly the given string and any whitespace after it.
        /// Uses the <see cref="Spaces"/> parser to parse subesquent whitespace.
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string and any whitespace afterwards </returns>
        /// <exception cref="ArgumentNullException"> If the provided string is null </exception>
        public static Parser<string, char> Symbol(string stringToParse)
        {
            if (stringToParse is null) throw new ArgumentNullException(nameof(stringToParse));

            return Token(String(stringToParse));
        }

        /// <summary>
        /// Returns a parser which parses exactly the given string and any whitespace after it
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <param name="spaceConsumer"> The parser for the whitespace after the string </param>
        /// <returns> Parser which parses exactly the given string and any whitespace afterwards </returns>
        /// <exception cref="ArgumentNullException"> If the provided string is null </exception>
        public static Parser<string, char> Symbol<TSpace>(string stringToParse, Parser<TSpace, char> spaceConsumer)
        {
            if (stringToParse is null) throw new ArgumentNullException(nameof(stringToParse));
            if (spaceConsumer is null) throw new ArgumentNullException(nameof(spaceConsumer));

            return Token(String(stringToParse), spaceConsumer);
        }
    }
}
