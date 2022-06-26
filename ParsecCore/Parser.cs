using System;
using System.Collections.Generic;

using ParsecCore.Parsers;
using ParsecCore.Help;

namespace ParsecCore
{
    public static class Parser
    {
        /// <summary>
        /// Parser which parses an end of file - end of input
        /// None is a helper struct which is empty - it only serves the type system
        /// </summary>
        public static readonly IParser<None> EOF = new EOFParser();

        /// <summary>
        /// Parser which parses any character
        /// </summary>
        public static readonly IParser<char> AnyChar = new AnyParser();

        /// <summary>
        /// Returns a parser which parses a char given it fulfills a predicate
        /// </summary>
        /// <param name="predicate"> The predicate the character must fulfill </param>
        /// <param name="description"> Description of the expected character for error messages </param>
        /// <returns> Parser which parses a character given it fulfills a predicate </returns>
        public static IParser<char> Satisfy(Predicate<char> predicate, string description) =>
            new SatisfyParser(predicate, description);

        /// <summary>
        /// Returns a parser which parses only the given character
        /// </summary>
        /// <param name="c"> The character to parse </param>
        /// <returns> Parser which parses only the given character </returns>
        public static IParser<char> Char(char c) =>
            new SatisfyParser(i => i == c, $"character {c}");

        /// <summary>
        /// Parses a single whitespace
        /// </summary>
        public static readonly IParser<char> Space = Satisfy(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parses as much whitespace as possible
        /// </summary>
        public static readonly IParser<string> Spaces = Space.Many();

        /// <summary>
        /// Parses a horizontal tab
        /// </summary>
        public static readonly IParser<char> Tab = Satisfy(c => c == '\t', "horizontal tab");

        /// <summary>
        /// Parses a newline (line feed) character
        /// </summary>
        public static readonly IParser<char> NewLine = Satisfy(c => c == '\n', "newline");

        /// <summary>
        /// Parses carriage return folled by line feed (\r\n), returns only line feed (\n)
        /// </summary>
        public static readonly IParser<char> CRLF =
            from cr in Satisfy(c => c == '\r', "carriage return")
            from lf in NewLine
            select '\n';


        /// <summary>
        /// Parses a single digit
        /// </summary>
        public static readonly IParser<char> Digit = Satisfy(c => c >= '0' && c <= '9', "digit");

        /// <summary>
        /// Parses as many digits as possible, but at least one
        /// </summary>
        public static readonly IParser<string> Digits = Digit.Many1();

        /// <summary>
        /// Parses an octal digit
        /// </summary>
        public static readonly IParser<char> OctDigit = Satisfy(c => c >= '0' && c <= '7', "octal digit");

        /// <summary>
        /// Parses a hexadecimal digit
        /// </summary>
        public static readonly IParser<char> HexDigit = Satisfy(
            c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'),
            "hexadecimal digit"
            );

        /// <summary>
        /// Parses a letter
        /// </summary>
        public static readonly IParser<char> Letter = Satisfy(char.IsLetter, "letter");

        /// <summary>
        /// Parses an alphanumeric character
        /// </summary>
        public static readonly IParser<char> AlphaNum = Satisfy(char.IsLetterOrDigit, "alphanumeric character");

        /// <summary>
        /// Parses an upper-case character
        /// </summary>
        public static readonly IParser<char> Upper = Satisfy(char.IsUpper, "upper-case character");

        /// <summary>
        /// Parses a lower-case character
        /// </summary>
        public static readonly IParser<char> Lower = Satisfy(char.IsLower, "lower-case character");

        public static IParser<char> OneOf(char[] chars)
        {
            throw new NotImplementedException();
        }

        public static IParser<char> NoneOf(char[] chars)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a parser which does not consume any input and only returns the value given
        /// </summary>
        /// <typeparam name="T"> The type of the value the parser returns </typeparam>
        /// <param name="value"> The value for the parser to return </param>
        /// <returns> Parser which returns the given value </returns>
        public static IParser<T> Return<T>(T value) =>
            new ReturnParser<T>(value);

        /// <summary>
        /// Returns a parser which parses exactly the string given
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string </returns>
        public static IParser<string> String(string stringToParse)
        {
            static IEnumerable<IParser<char>> stringToCharParsers(string str)
            {
                IParser<char>[] parsers = new IParser<char>[str.Length];
                for (int i = 0; i < str.Length; ++i)
                {
                    parsers[i] = Char(str[i]);
                }
                return parsers;
            }

            return from chars in new AllParser<char>(stringToCharParsers(stringToParse))
                   select string.Concat(chars);
        }

        /// <summary>
        /// Returns a parser which ignores any whitespace before or after the parsed value
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <param name="parser"> What value to parse between the whitespace </param>
        /// <returns>
        /// Parser which parses the same value as the input parser surrounded by an arbitrary amount of whitespace.
        /// </returns>
        public static IParser<T> Token<T>(IParser<T> parser) => 
            Combinator.Between(Spaces, parser);

        /// <summary>
        /// Returns a parser which parses exactly the given string and any whitespace before or after it
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string and any whitespace afterwards </returns>
        public static IParser<string> Symbol(string stringToParse) => 
            Token(String(stringToParse));
    }
}
