using ParsecCore.Help;
using System.Collections.Generic;
using System;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Parse seperated values. Parses a list of values each of which is seperated from the other.
        /// Usefull in parsing lists (values would be for example integers and the seperator a string ",")
        /// </summary>
        /// <typeparam name="TValue"> The type of the parsed values </typeparam>
        /// <typeparam name="TSeparator"> The type of the seperator </typeparam>
        /// <param name="valueParser"> Parser for the values </param>
        /// <param name="separatorParser"> Parser for the seperators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<IReadOnlyList<TValue>, TInputToken> SepBy<TValue, TSeparator, TInputToken>(
            Parser<TValue, TInputToken> valueParser,
            Parser<TSeparator, TInputToken> separatorParser
        ) =>
            Choice(
                SepBy1(valueParser, separatorParser),
                Return<IReadOnlyList<TValue>, TInputToken>(Array.Empty<TValue>())
            );

        /// <summary>
        /// Parse seperated values. Parses a list of values each of which is separated from the other.
        /// Usefull in parsing lists (values would be for example integers and the seperator a string ",")
        /// Always parses at least one value.
        /// </summary>
        /// <typeparam name="TValue"> The type of the parsed values </typeparam>
        /// <typeparam name="TSeparator"> The type of the seperator </typeparam>
        /// <param name="valueParser"> Parser for the values </param>
        /// <param name="separatorParser"> Parser for the seperators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<IReadOnlyList<TValue>, TInputToken> SepBy1<TValue, TSeparator, TInputToken>(
            Parser<TValue, TInputToken> valueParser,
            Parser<TSeparator, TInputToken> separatorParser
        )
        {
            if (valueParser is null) throw new ArgumentNullException(nameof(valueParser));
            if (separatorParser is null) throw new ArgumentNullException(nameof(separatorParser));

            var sepValueParser = separatorParser.Then(valueParser);

            return from firstParse in valueParser
                   from subsequentParses in sepValueParser.Many()
                   select subsequentParses.Prepend(firstParse);
        }

        /// <summary>
        /// Parses seperated values. Parses a list of values separeted by separators
        /// and ended by the separator.
        /// </summary>
        /// <typeparam name="TValue"> The type of the value parser </typeparam>
        /// <typeparam name="TSeparator"> The type of the separator parser </typeparam>
        /// <param name="valueParser"> The parser for the values </param>
        /// <param name="separatorParser"> The parser for the separators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<IReadOnlyList<TValue>, TInputToken> EndBy<TValue, TSeparator, TInputToken>(
            Parser<TValue, TInputToken> valueParser,
            Parser<TSeparator, TInputToken> separatorParser
        )
        {
            if (valueParser is null) throw new ArgumentNullException(nameof(valueParser));
            if (separatorParser is null) throw new ArgumentNullException(nameof(separatorParser));

            return valueParser.FollowedBy(separatorParser).Many();
        }

        /// <summary>
        /// Parses seperated values. Parses a list of values separeted by separators
        /// and ended by the separator.
        /// Parses at least one value
        /// </summary>
        /// <typeparam name="TValue"> The type of the value parser </typeparam>
        /// <typeparam name="TSeparator"> The type of the separator parser </typeparam>
        /// <param name="valueParser"> The parser for the values </param>
        /// <param name="separatorParser"> The parser for the separators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<IReadOnlyList<TValue>, TInputToken> EndBy1<TValue, TSeparator, TInputToken>(
            Parser<TValue, TInputToken> valueParser,
            Parser<TSeparator, TInputToken> separatorParser
        )
        {
            if (valueParser is null) throw new ArgumentNullException(nameof(valueParser));
            if (separatorParser is null) throw new ArgumentNullException(nameof(separatorParser));

            return valueParser.FollowedBy(separatorParser).Many1();
        }

        /// <summary>
        /// Parses list of seperated values which optionally end with a separator.
        /// </summary>
        /// <typeparam name="TValue"> The type of the value parser </typeparam>
        /// <typeparam name="TSeparator"> The type of the separator parser </typeparam>
        /// <param name="valueParser"> The parser for the values </param>
        /// <param name="separatorParser"> The parser for the separators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<IReadOnlyList<TValue>, TInputToken> SepEndBy<TValue, TSeparator, TInputToken>(
            Parser<TValue, TInputToken> valueParser,
            Parser<TSeparator, TInputToken> separatorParser
        ) =>
            Choice(SepBy(valueParser, separatorParser).Try(), EndBy(valueParser, separatorParser));

        /// <summary>
        /// Parses list of seperated values which optionally end with a separator.
        /// Always parses at least one value.
        /// </summary>
        /// <typeparam name="TValue"> The type of the value parser </typeparam>
        /// <typeparam name="TSeparator"> The type of the separator parser </typeparam>
        /// <param name="valueParser"> The parser for the values </param>
        /// <param name="separatorParser"> The parser for the separators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<IReadOnlyList<TValue>, TInputToken> SepEndBy1<TValue, TSeparator, TInputToken>(
            Parser<TValue, TInputToken> valueParser,
            Parser<TSeparator, TInputToken> separatorParser
        ) =>
            Choice(SepBy1(valueParser, separatorParser).Try(), EndBy1(valueParser, separatorParser));

    }
}
