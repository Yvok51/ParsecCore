using ParsecCore.ParsersHelp;
using System;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Returns a parser which either parses its value or returns 
        /// Nothing if the underlying parser does not consume any input or propagates
        /// the error if any input was consumed.
        /// See <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> to change this beavior.
        /// </summary>
        /// <typeparam name="T"> The result type of the parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to optionally apply </param>
        /// <returns> Parser which optionally applies the given parser </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<Maybe<T>, TInputToken> Optional<T, TInputToken>(
            this Parser<T, TInputToken> parser
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            return OptionalParser.Parser(parser);
        }

        /// <summary>
        /// Tries to parse according to the given parser. If the parser succeeds, then returns the parser's result.
        /// If the parser fails <strong>without consuming input</strong>, then returns the given default value.
        /// If the parser fails while consuming input, the failure is propagated upwards.
        /// See <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> to change this behavior
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to modify </param>
        /// <param name="defaultValue"> The default value to return if the parser fails </param>
        /// <returns> Parser which returns a default value upon failure </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, TInputToken> Option<T, TInputToken>(
            this Parser<T, TInputToken> parser,
            T defaultValue
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (defaultValue is null) throw new ArgumentNullException(nameof(defaultValue));

            return from opt in parser.Optional()
                   select opt.Else(defaultValue);
        }
    }
}
