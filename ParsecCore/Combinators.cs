using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ParsecCore.ParsersHelp;

namespace ParsecCore
{
    public static class Combinators
    {
        /// <summary>
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails then the second parser is applied on the same input as the first and so on.
        /// If all parsers fail then returns the ParseError of tha last parser
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns> Parser which sequentally tries to apply the given parsers until one succeeds or all fails </returns>
        public static Parser<T> Choice<T>(params Parser<T>[] parsers) =>
            ChoiceParser.Parser(parsers);

        /// <summary>
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails then the second parser is applied on the same input as the first and so on.
        /// If all parsers fail then returns the ParseError of tha last parser
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns> Parser which sequentially tries to apply the given parsers until one succeeds or all fails </returns>
        public static Parser<T> Choice<T>(IEnumerable<Parser<T>> parsers) =>
            ChoiceParser.Parser(parsers);

        /// <summary>
        /// Returns a parser which tries to parse all of the given parsers in a sequence.
        /// If it succeeds it returns an IEnumerable of the parsed results.
        /// If it fails it returns the first encountered parse error
        /// </summary>
        /// <typeparam name="T"> The type of parsers </typeparam>
        /// <param name="parsers"> Parsers to sequentially apply </param>
        /// <returns> Parser which sequentially aplies all of the given parsers </returns>
        public static Parser<IEnumerable<T>> All<T>(params Parser<T>[] parsers) =>
            AllParser.Parser(parsers);

        /// <summary>
        /// Returns a parser which tries to parse all of the given parsers in a sequence.
        /// If it succeeds it returns an IEnumerable of the parsed results.
        /// If it fails it returns the first encountered parse error
        /// </summary>
        /// <typeparam name="T"> The type of parsers </typeparam>
        /// <param name="parsers"> Parsers to sequentially apply </param>
        /// <returns> Parser which sequentially aplies all of the given parsers </returns>
        public static Parser<IEnumerable<T>> All<T>(IEnumerable<Parser<T>> parsers) =>
            AllParser.Parser(parsers);

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
        public static Parser<TBetween> Between<TLeft, TBetween, TRight>(
            Parser<TLeft> leftParser,
            Parser<TBetween> betweenParser,
            Parser<TRight> rightParser
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
        public static Parser<TBetween> Between<TOutside, TBetween>(
            Parser<TOutside> outsideParser,
            Parser<TBetween> betweenParser
        ) =>
            Between(outsideParser, betweenParser, outsideParser);

        /// <summary>
        /// Parse seperated values. Parses a list of values each of which is seperated from the other.
        /// Usefull in parsing lists (values would be for example integers and the seperator a string ",")
        /// </summary>
        /// <typeparam name="TValue"> The type of the parsed values </typeparam>
        /// <typeparam name="TSeparator"> The type of the seperator </typeparam>
        /// <param name="valueParser"> Parser for the values </param>
        /// <param name="separatorParser"> Parser for the seperators </param>
        /// <returns> Parser which returns a list of parsed values </returns>
        public static Parser<IEnumerable<TValue>> SepBy<TValue, TSeparator>(
            Parser<TValue> valueParser,
            Parser<TSeparator> separatorParser
        ) =>
            Choice(SepBy1(valueParser, separatorParser), Parsers.Return<IEnumerable<TValue>>(Array.Empty<TValue>()));

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
        public static Parser<IEnumerable<TValue>> SepBy1<TValue, TSeparator>(
            Parser<TValue> valueParser,
            Parser<TSeparator> separatorParser
        ) =>
            SepBy1Parser.Parser<TValue, TSeparator>(valueParser, separatorParser);
    }
}
