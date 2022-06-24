using System;
using System.Collections.Generic;

using ParsecCore.Parsers;
using ParsecCore.HelpParsers;
using ParsecCore.Help;

namespace ParsecCore
{
    public static class Parser
    {
        /// <summary>
        /// Parser which parses an end of file - end of input
        /// None is a helper struct which is empty - it only serves the type system
        /// </summary>
        public static IParser<None> EOF = new EOFParser();

        /// <summary>
        /// Parser which parses any character
        /// </summary>
        public static IParser<char> AnyChar = new AnyParser();

        /// <summary>
        /// Returns a parser which parses only the given character
        /// </summary>
        /// <param name="c"> The character to parse </param>
        /// <returns> Parser which parses only the given character </returns>
        public static IParser<char> Char(char c) =>
            new CharParser(c);

        /// <summary>
        /// Returns a parser which parses a char given it fulfills a predicate
        /// </summary>
        /// <param name="predicate"> The predicate the character must fulfill </param>
        /// <param name="description"> Description of the expected character for error messages </param>
        /// <returns> Parser which parses a character given it fulfills a predicate </returns>
        public static IParser<char> Satisfy(Predicate<char> predicate, string description) =>
            new SatisfyParser(predicate, description);

        /// <summary>
        /// Parses a single whitespace
        /// </summary>
        public static IParser<char> Whitespace = Satisfy(char.IsWhiteSpace, "whitespace");
        /// <summary>
        /// Parses as much whitespace as possible
        /// </summary>
        public static IParser<string> Spaces = Whitespace.Many();
        /// <summary>
        /// Parses a single digit
        /// </summary>
        public static IParser<char> Digit = Satisfy(char.IsDigit, "digit");
        /// <summary>
        /// Parses as many digits as possible, but at least one
        /// </summary>
        public static IParser<string> Digits = Digit.Many1();

        /// <summary>
        /// Return a parser which does not consume any input and only returns the value given
        /// </summary>
        /// <typeparam name="T"> The type of the value the parser returns </typeparam>
        /// <param name="value"> The value for the parser to return </param>
        /// <returns> Parser which returns the given value </returns>
        public static IParser<T> Return<T>(T value) =>
            new ReturnParser<T>(value);

        /// <summary>
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails then the second parser is applied on the same input as the first and so on.
        /// If all parsers fail then returns the ParseError of tha last parser
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns> Parser which sequentally tries to apply the given parsers until one succeeds or all fails </returns>
        public static IParser<T> Choice<T>(params IParser<T>[] parsers) =>
            new ChoiceParser<T>(parsers);

        /// <summary>
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails then the second parser is applied on the same input as the first and so on.
        /// If all parsers fail then returns the ParseError of tha last parser
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns> Parser which sequentially tries to apply the given parsers until one succeeds or all fails </returns>
        public static IParser<T> Choice<T>(IEnumerable<IParser<T>> parsers) =>
            new ChoiceParser<T>(parsers);

        /// <summary>
        /// Returns a parser which tries to parse all of the given parsers in a sequence.
        /// If it succeeds it returns an IEnumerable of the parsed results.
        /// If it fails it returns the first encountered parse error
        /// </summary>
        /// <typeparam name="T"> The type of parsers </typeparam>
        /// <param name="parsers"> Parsers to sequentially apply </param>
        /// <returns> Parser which sequentially aplies all of the given parsers </returns>
        public static IParser<IEnumerable<T>> All<T>(params IParser<T>[] parsers) =>
            new AllParser<T>(parsers);

        /// <summary>
        /// Returns a parser which tries to parse all of the given parsers in a sequence.
        /// If it succeeds it returns an IEnumerable of the parsed results.
        /// If it fails it returns the first encountered parse error
        /// </summary>
        /// <typeparam name="T"> The type of parsers </typeparam>
        /// <param name="parsers"> Parsers to sequentially apply </param>
        /// <returns> Parser which sequentially aplies all of the given parsers </returns>
        public static IParser<IEnumerable<T>> All<T>(IEnumerable<IParser<T>> parsers) =>
            new AllParser<T>(parsers);

        /// <summary>
        /// Returns a parser which parses exactly the string given
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string </returns>
        public static IParser<string> String(string stringToParse)
        {
            Func<string, IEnumerable<IParser<char>>> stringToCharParsers = (string str) =>
            {
                IParser<char>[] parsers = new IParser<char>[str.Length];
                for (int i = 0; i < str.Length; ++i)
                {
                    parsers[i] = Char(str[i]);
                }
                return parsers;
            };

            return from chars in new AllParser<char>(stringToCharParsers(stringToParse))
                   select string.Concat(chars);
        }

        /// <summary>
        /// Returns a parser which parses exactly the given string and any whitespace afterwards
        /// </summary>
        /// <param name="stringToParse"> The string for the parser to parse </param>
        /// <returns> Parser which parses exactly the given string and any whitespace afterwards </returns>
        public static IParser<string> Symbol(string stringToParse)
        {
            return from str in String(stringToParse)
                   from space in Spaces
                   select str;
        }

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
        /// Parser which parses the entire sequence of leftParse-betweenParser-rightParser but only
        /// returns the value parsed by the betweenParser
        /// </returns>
        public static IParser<TBetween> Between<TLeft, TBetween, TRight>(
            IParser<TLeft> leftParser,
            IParser<TBetween> betweenParser,
            IParser<TRight> rightParser
        )
        {
            return from l in leftParser
                   from between in betweenParser
                   from r in rightParser
                   select between;
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
        /// Parser which parses the entire sequence of outsideParser-betweenParser-outsideParser but only
        /// returns the value parsed by the betweenParser
        /// </returns>
        public static IParser<TBetween> Between<TOutside, TBetween>(
            IParser<TOutside> outsideParser,
            IParser<TBetween> betweenParser
        )
        {
            return Between(outsideParser, betweenParser, outsideParser);
        }

        /// <summary>
        /// Parse seperated values. Parses a list of values each of which is seperated from the other.
        /// Usefull in parsing lists (values would be for example integers and the seperator a string ",")
        /// </summary>
        /// <typeparam name="TValue"> The type of the parsed values </typeparam>
        /// <typeparam name="TSeparator"> The type of the seperator </typeparam>
        /// <param name="valueParser"> Parser for the values </param>
        /// <param name="separatorParser"> Parser for the seperators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        public static IParser<IEnumerable<TValue>> SepBy<TValue, TSeparator>(
            IParser<TValue> valueParser,
            IParser<TSeparator> separatorParser
        )
        {
            return Choice(SepBy1(valueParser, separatorParser), Return<IEnumerable<TValue>>(Array.Empty<TValue>()));
        }

        /// <summary>
        /// Parse seperated values. Parses a list of values each of which is seperated from the other.
        /// Usefull in parsing lists (values would be for example integers and the seperator a string ",")
        /// Always parses at least one value.
        /// </summary>
        /// <typeparam name="TValue"> The type of the parsed values </typeparam>
        /// <typeparam name="TSeparator"> The type of the seperator </typeparam>
        /// <param name="valueParser"> Parser for the values </param>
        /// <param name="separatorParser"> Parser for the seperators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        public static IParser<IEnumerable<TValue>> SepBy1<TValue, TSeparator>(
            IParser<TValue> valueParser,
            IParser<TSeparator> separatorParser
)
        {
            return new HelpParsers.SepBy1Parser<TValue, TSeparator>(valueParser, separatorParser);
        }
    }
}
