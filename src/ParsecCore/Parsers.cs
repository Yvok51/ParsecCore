using ParsecCore.EitherNS;
using ParsecCore.Help;
using ParsecCore.Indentation;
using ParsecCore.Input;
using ParsecCore.MaybeNS;
using ParsecCore.ParsersHelp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Parser which parses an end of file - end of input.
        /// If the input has not ended, then fails.
        /// Used to check we parsed the entire input.
        /// <see cref="None"/> is a helper struct which is empty - it only serves the type system
        /// </summary>
        /// <typeparam name="TInput"> The token type of the input </typeparam>
        /// <returns> Parser which parses an end of file </returns>
        public static Parser<None, TInput> EOF<TInput>()
            => EOFParser.Parser<TInput>();

        /// <summary>
        /// Parser which answers whether we have reached the end of file.
        /// It always succeeds and returns a boolean value signifying the answer.
        /// True - there is no more input. False - there is more input yet.
        /// </summary>
        /// <typeparam name="TInput"> The token type of the input </typeparam>
        /// <returns> Boolean value whether we are at the end of file </returns>
        public static Parser<bool, TInput> IsEOF<TInput>() =>
            from end in EOF<TInput>().Optional()
            select !end.IsEmpty;

        /// <summary>
        /// Returns a parser which parses a char given it fulfills a predicate.
        /// Consumes the read character only if the character passes the predicate.
        /// </summary>
        /// <param name="predicate"> The predicate the character must fulfill </param>
        /// <param name="description"> Description of the expected character for error messages </param>
        /// <returns> Parser which parses a character given it fulfills a predicate </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<char, char> Satisfy(
            Predicate<char> predicate,
            string description
        )
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (description is null) throw new ArgumentNullException(nameof(description));

            return SatisfyParser.Parser(predicate, description);
        }

        /// <summary>
        /// Returns a parser which parses a token given it fulfills a predicate.
        /// Consumes the read token only if the token passes the predicate.
        /// </summary>
        /// <param name="predicate"> The predicate the token must fulfill </param>
        /// <param name="description"> Description of the expected token for error messages </param>
        /// <typeparam name="T"> 
        /// The result type of the parser. Given the semantics of the parser also the input type of the parser.
        /// </typeparam>
        /// <returns> Parser which parses an input token given it fulfills a predicate </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, T> Satisfy<T>(
            Predicate<T> predicate,
            string description
        )
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (description is null) throw new ArgumentNullException(nameof(description));

            return SatisfyParser.Parser(predicate, description);
        }

        /// <summary>
        /// Parser which parses any character
        /// </summary>
        public static readonly Parser<char, char> AnyChar = Satisfy(c => true, "any character");

        /// <summary>
        /// Parser which parses any token
        /// </summary>
        public static Parser<T, T> AnyToken<T>() => Satisfy<T>(c => true, "any character");

        /// <summary>
        /// Returns a parser which parses only the given character.
        /// Does not consume any input upon failure.
        /// </summary>
        /// <param name="c"> The character to parse </param>
        /// <returns> Parser which parses only the given character </returns>
        public static Parser<char, char> Char(char c) =>
            SatisfyParser.Parser(c, escapedChars.ContainsKey(c) ? $"character '{escapedChars[c]}'" : $"character '{c}'");

        /// <summary>
        /// Returns a parser which parses only the given token.
        /// Does not consume any input upon failure.
        /// </summary>
        /// <param name="token"> The token to parse </param>
        /// <returns> Parser which parses only the given token </returns>
        public static Parser<T, T> Char<T>(T token) =>
            SatisfyParser.Parser(token, $"token '{token}'");

        private static Dictionary<char, string> escapedChars = new Dictionary<char, string>()
        {
            { '\n', "\\n" },
            { '\b', "\\b" },
            { '\f', "\\f" },
            { '\r', "\\r" },
            { '\t', "\\t" },
            { '\v', "\\v" },
        };

        /// <summary>
        /// Parses a single whitespace, uses <see cref="char.IsWhiteSpace(char)"/> to determine whitespace
        /// </summary>
        public static readonly Parser<char, char> Space = Satisfy(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parses as much whitespace as possible, uses <see cref="Space"/> to parse whitespace
        /// </summary>
        public static readonly Parser<string, char> Spaces = Space.Many();

        /// <summary>
        /// Parses a horizontal tab
        /// </summary>
        public static readonly Parser<char, char> Tab = Satisfy(c => c == '\t', "horizontal tab");

        /// <summary>
        /// Parses a newline (line feed, '\n') character
        /// </summary>
        public static readonly Parser<char, char> NewLine = Satisfy(c => c == '\n', "newline");

        /// <summary>
        /// Parses a carriage return ('\r') character
        /// </summary>
        public static readonly Parser<char, char> CarriageReturn = Satisfy(c => c == '\r', "carriage return");

        /// <summary>
        /// Parses carriage return folled by line feed (\r\n), returns only line feed (\n)
        /// </summary>
        public static readonly Parser<char, char> CRLF =
            from cr in CarriageReturn
            from lf in NewLine
            select lf;

        /// <summary>
        /// Parses end of line ('\n', '\r' or '\r\n') and returns '\n'
        /// </summary>
        public static readonly Parser<char, char> EOL = // made so that we don't need lookahead
            NewLine.Or(
                CarriageReturn.Then(NewLine.Or(Return<char, char>('\n')))
            );

        /// <summary>
        /// Parses a single digit
        /// </summary>
        public static readonly Parser<char, char> Digit = Satisfy(c => c >= '0' && c <= '9', "digit");

        /// <summary>
        /// Parses as many digits as possible, but at least one
        /// </summary>
        public static readonly Parser<string, char> Digits = Digit.Many1();

        /// <summary>
        /// Parses an integer
        /// </summary>
        public static readonly Parser<int, char> DecimalInteger =
            from op in Symbol("-").Or(Symbol("+")).Option(string.Empty)
            from digits in Token(Digits)
            select Int32.Parse(op + digits);

        /// <summary>
        /// Parses an octal digit
        /// </summary>
        public static readonly Parser<char, char> OctDigit = Satisfy(c => c >= '0' && c <= '7', "octal digit");

        /// <summary>
        /// Parses a hexadecimal digit
        /// </summary>
        public static readonly Parser<char, char> HexDigit = Satisfy(
            c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'),
            "hexadecimal digit"
            );

        /// <summary>
        /// Parses a letter
        /// </summary>
        public static readonly Parser<char, char> Letter = Satisfy(char.IsLetter, "letter");

        /// <summary>
        /// Parses an alphanumeric character
        /// </summary>
        public static readonly Parser<char, char> AlphaNum = Satisfy(char.IsLetterOrDigit, "alphanumeric character");

        /// <summary>
        /// Parses an upper-case character
        /// </summary>
        public static readonly Parser<char, char> Upper = Satisfy(char.IsUpper, "upper-case character");

        /// <summary>
        /// Parses a lower-case character
        /// </summary>
        public static readonly Parser<char, char> Lower = Satisfy(char.IsLower, "lower-case character");

        private static IEnumerable<Parser<char, char>> ToCharParsers(IEnumerable<char> chars) =>
            chars.Select(c => Char(c));

        /// <summary>
        /// Parses any of the given characters
        /// </summary>
        /// <param name="chars"> Possible characters to parse </param>
        /// <returns> Parser which only parses one of the given characters </returns>
        /// <exception cref="ArgumentNullException"> If provided array is null </exception>
        public static Parser<char, char> OneOf(params char[] chars)
        {
            if (chars is null) throw new ArgumentNullException(nameof(chars));

            return Satisfy(c =>
            {
                foreach (char includedChar in chars)
                {
                    if (c == includedChar)
                    {
                        return true;
                    }
                }
                return false;
            },
            $"one of {chars.ToPrettyString()}");
        }

        /// <summary>
        /// Parses any of the given tokens
        /// </summary>
        /// <param name="tokens"> Possible tokens to parse </param>
        /// <typeparam name="T"> The parser and input type of the parser </typeparam>
        /// <returns> Parser which only parses one of the given tokens </returns>
        /// <exception cref="ArgumentNullException"> If provided array is null </exception>
        public static Parser<T, T> OneOf<T>(params T[] tokens)
        {
            if (tokens is null) throw new ArgumentNullException(nameof(tokens));

            return Satisfy<T>(token =>
            {
                foreach (T includedToken in tokens)
                {
                    if (EqualityComparer<T>.Default.Equals(includedToken, token))
                    {
                        return true;
                    }
                }
                return false;
            },
            $"one of {tokens.ToPrettyString()}");
        }

        /// <summary>
        /// Parses a character if it is not equal to any of the given characters
        /// </summary>
        /// <param name="chars"> The list of characters the read character must not be in </param>
        /// <returns> Parser which parses a character only if it is not included in the given list </returns>
        /// <exception cref="ArgumentNullException"> If provided array is null </exception>
        public static Parser<char, char> NoneOf(params char[] chars)
        {
            if (chars is null) throw new ArgumentNullException(nameof(chars));

            return Satisfy(c =>
            {
                foreach (char excludedChar in chars)
                {
                    if (c == excludedChar)
                    {
                        return false;
                    }
                }
                return true;
            },
            $"none of {chars.ToPrettyString()}");
        }

        /// <summary>
        /// Parses a character if it is not equal to any of the given tokens
        /// </summary>
        /// <param name="tokens"> The list of characters the read character must not be in </param>
        /// <typeparam name="T"> The parser and input type of the parser </typeparam>
        /// <returns> Parser which parses a character only if it is not included in the given list </returns>
        /// <exception cref="ArgumentNullException"> If provided array is null </exception>
        public static Parser<T, T> NoneOf<T>(params T[] tokens)
        {
            if (tokens is null) throw new ArgumentNullException(nameof(tokens));

            return Satisfy<T>(token =>
            {
                foreach (T excludedToken in tokens)
                {
                    if (EqualityComparer<T>.Default.Equals(excludedToken, token))
                    {
                        return false;
                    }
                }
                return true;
            },
            $"none of {tokens.ToPrettyString()}");
        }

        /// <summary>
        /// Return a parser which does not consume any input and only returns the value given
        /// </summary>
        /// <typeparam name="T"> The type of the value the parser returns </typeparam>
        /// <typeparam name="TInputToken"> The input type of the parser </typeparam>
        /// <param name="value"> The value for the parser to return </param>
        /// <returns> Parser which returns the given value </returns>
        public static Parser<T, TInputToken> Return<T, TInputToken>(T value)
        {
            return (input) =>
            {
                return Either.Result<ParseError, T>(value);
            };
        }

        /// <summary>
        /// Parser which returns the current position of the input
        /// </summary>
        /// <typeparam name="TInputToken"> The input type of the parser </typeparam>
        /// <returns></returns>
        public static Parser<Position, TInputToken> Position<TInputToken>()
        {
            return (input) =>
            {
                return Either.Result<ParseError, Position>(input.Position);
            };
        }

        #region Error Handling

        /// <summary>
        /// Returns a parser which always fails and returns the given error.
        /// Does not consume any input.
        /// </summary>
        /// <typeparam name="T"> The result type of the parser </typeparam>
        /// <typeparam name="TInputToken"> The input type of the parser </typeparam>
        /// <param name="error"> The error to return </param>
        /// <returns> Parser that always fails with the given error </returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="error"/> is null </exception>
        public static Parser<T, TInputToken> ParserError<T, TInputToken>(ParseError error)
        {
            if (error is null) throw new ArgumentNullException(nameof(error));

            return (input) =>
            {
                return Either.Error<ParseError, T>(error);
            };
        }

        /// <summary>
        /// Returns a parser which always fails with the given message.
        /// Does not consume any input.
        /// <para>
        /// The error that is raised is <see cref="CustomError"/>.
        /// </para>
        /// </summary>
        /// <typeparam name="T"> The result type of parser </typeparam>
        /// <typeparam name="TInputToken"> The input type of the parser </typeparam>
        /// <param name="msg"> The message to fail with </param>
        /// <returns> Parser that always fails with the given message </returns>
        /// <exception cref="ArgumentNullException"> If <paramref name="msg"/> is null </exception>
        public static Parser<T, TInputToken> Fail<T, TInputToken>(string msg)
        {
            if (msg is null) throw new ArgumentNullException(nameof(msg));

            return from pos in Position<TInputToken>()
                   from err in ParserError<T, TInputToken>(new CustomError(pos, new FailWithError(msg)))
                   select err;
        }

        #endregion

        /// <summary>
        /// Returns a parser which parses exactly the string given
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string </returns>
        /// <exception cref="ArgumentNullException"> If provided string is null </exception>
        public static Parser<string, char> String(string stringToParse)
        {
            if (stringToParse is null) throw new ArgumentNullException(nameof(stringToParse));

            var stringParser = AllParser.Parser(ToCharParsers(stringToParse));
            return from chars in stringParser
                   select string.Concat(chars);
        }

        /// <summary>
        /// Sequence many parsers. This combinator tries to parse all of the given parsers in a sequence.
        /// If any of the parsers fail, then the entire parser fails.
        /// Returns the list of results.
        /// </summary>
        /// <typeparam name="T"> The result type of the parsers to sequence </typeparam>
        /// <typeparam name="TInputToken"> The input of the parsers </typeparam>
        /// <param name="parsers"> The parsers to sequence </param>
        /// <returns> Parser that parses all given parsers in a sequence</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Parser<IReadOnlyList<T>, TInputToken> Sequence<T, TInputToken>(
            IEnumerable<Parser<T, TInputToken>> parsers
        )
        {
            if (parsers is null) throw new ArgumentNullException(nameof(parsers));

            return AllParser.Parser(parsers);
        }

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

            return from x in parser
                   from _ in Spaces
                   select x;
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

            return from x in parser
                   from _ in spaceConsumer
                   select x;
        }

        /// <summary>
        /// Returns a parser which parses exactly the given string and any whitespace before or after it
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
        /// Make the creation of the parser indirect.
        /// <para/>
        /// Used when a circular compile time dependency occurs between the parsers. 
        /// In such a case value of one of the parsers is always initialized only after being used in another parser.
        /// The value is thus null and an error occurs.
        /// <para/>
        /// We solve this by indirectly initializing one of the parsers, thus the value of the parser with whom
        /// we are circularly dependent is taken after it has been initialized and thus everything works.
        /// We do this by putting the parser into a lambda function. We postpone the creation of the parser to the
        /// runtime and therefore the other parser on which we are dependent is already defined.
        /// <para/>
        /// Error will occur if the grammar with the circularly dependent parsers is left recursive. In that case
        /// the indirection will go on until a stack overflow.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <typeparam name="TInputToken"> The input type of the parser </typeparam>
        /// <param name="getParser"> Function to get the parser from </param>
        /// <returns> Same parser as returned by <paramref name="getParser"/> only behaving correctly </returns>
        /// <exception cref="ArgumentNullException"> If provided function is null </exception>
        public static Parser<T, TInputToken> Indirect<T, TInputToken>(
            Func<Parser<T, TInputToken>> getParser
        )
        {
            if (getParser is null) throw new ArgumentNullException(nameof(getParser));

            return (input) => getParser()(input);
        }
    }
}
