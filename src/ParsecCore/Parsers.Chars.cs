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
    }
}
