﻿using ParsecCore.EitherNS;
using ParsecCore.Help;
using ParsecCore.MaybeNS;
using ParsecCore.ParsersHelp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsecCore
{
    public static class Parsers
    {
        /// <summary>
        /// Parser which parses an end of file - end of input
        /// None is a helper struct which is empty - it only serves the type system
        /// </summary>
        public static readonly Parser<None, char> EOF = EOFParser.Parser<char>();

        /// <summary>
        /// Parser which answers whether we have reached the end of file.
        /// It always succeeds and returns a boolean value signifying the answer
        /// </summary>
        public static readonly Parser<bool, char> IsEOF = 
            from end in EOF.Optional()
            select !end.IsEmpty;

        /// <summary>
        /// Returns a parser which parses a char given it fulfills a predicate.
        /// Consumes the read character only if the character passes the predicate.
        /// </summary>
        /// <param name="predicate"> The predicate the character must fulfill </param>
        /// <param name="description"> Description of the expected character for error messages </param>
        /// <returns> Parser which parses a character given it fulfills a predicate </returns>
        public static Parser<char, char> Satisfy(Predicate<char> predicate, string description) =>
            SatisfyParser.Parser(predicate, description);

        /// <summary>
        /// Parser which parses any character
        /// </summary>
        public static readonly Parser<char, char> AnyChar = Satisfy(c => true, "any character");

        /// <summary>
        /// Returns a parser which parses only the given character.
        /// Does not consume any input upon failure.
        /// </summary>
        /// <param name="c"> The character to parse </param>
        /// <returns> Parser which parses only the given character </returns>
        public static Parser<char, char> Char(char c) =>
            Satisfy(i => i == c, escapedChars.ContainsKey(c) ? $"character '{escapedChars[c]}'" : $"character '{c}'");

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
        /// Parses a single whitespace
        /// </summary>
        public static readonly Parser<char, char> Space = Satisfy(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parses as much whitespace as possible
        /// </summary>
        public static readonly Parser<string, char> Spaces = Space.Many();

        /// <summary>
        /// Parses a horizontal tab
        /// </summary>
        public static readonly Parser<char, char> Tab = Satisfy(c => c == '\t', "horizontal tab");

        /// <summary>
        /// Parses a newline (line feed) character
        /// </summary>
        public static readonly Parser<char, char> NewLine = Satisfy(c => c == '\n', "newline");

        /// <summary>
        /// Parses carriage return folled by line feed (\r\n), returns only line feed (\n)
        /// </summary>
        public static readonly Parser<char, char> CRLF =
            from cr in Satisfy(c => c == '\r', "carriage return")
            from lf in NewLine
            select '\n';

        /// <summary>
        /// Parses end of line ('\n' or '\r\n') and returns '\n'
        /// </summary>
        public static readonly Parser<char, char> EOL = CRLF.Or(NewLine);

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
            from op in Combinators.Choice(Symbol("-"), Symbol("+")).Option(string.Empty)
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
        public static Parser<char, char> OneOf(char[] chars) =>
            Satisfy(c =>
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

        /// <summary>
        /// Parses a character if it is not equal to any of the given characters
        /// </summary>
        /// <param name="chars"> The list of characters the read character must not be in </param>
        /// <returns> Parser which parses a character only if it is not included in the given list </returns>
        public static Parser<char, char> NoneOf(char[] chars) =>
            Satisfy(c =>
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

        /// <summary>
        /// Return a parser which does not consume any input and only returns the value given
        /// </summary>
        /// <typeparam name="T"> The type of the value the parser returns </typeparam>
        /// <param name="value"> The value for the parser to return </param>
        /// <returns> Parser which returns the given value </returns>
        public static Parser<T, TInputToken> Return<T, TInputToken>(T value) =>
            (input) => Either.Result<ParseError, T>(value);

        /// <summary>
        /// Returns a parser which always fails with the given message.
        /// Does not consume any input.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <param name="msg"> The message to fail with </param>
        /// <returns> Parser which always fails with the given message </returns>
        public static Parser<T, TInputToken> Fail<T, TInputToken>(string msg) =>
            (input) => Either.Error<ParseError, T>(new ParseError(msg, input.Position));

        /// <summary>
        /// Returns a parser which parses exactly the string given
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string </returns>
        public static Parser<string, char> String(string stringToParse)
        {
            return from chars in AllParser.Parser(ToCharParsers(stringToParse))
                   select string.Concat(chars);
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
        public static Parser<T, char> Token<T>(Parser<T, char> parser)
        {
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
        public static Parser<T, TInput> Token<T, TSpace, TInput>(
            Parser<T, TInput> parser,
            Parser<TSpace, TInput> spaceConsumer
        )
        {
            return from x in parser
                   from _ in spaceConsumer
                   select x;
        }

        /// <summary>
        /// Returns a parser which parses exactly the given string and any whitespace before or after it
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string and any whitespace afterwards </returns>
        public static Parser<string, char> Symbol(string stringToParse) =>
            Token(String(stringToParse));

        /// <summary>
        /// Make the creation of the parser indirect.
        /// <para/>
        /// Used when a circular compile time dependency occurs between the parsers. In such a case value of one of the
        /// parsers is always initialized only after being used in another parser. The value is thus null and an error
        /// occurs.
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
        public static Parser<T, TInputToken> Indirect<T, TInputToken>(Func<Parser<T, TInputToken>> getParser)
        {
            return (input) => getParser()(input);
        }
    }
}
