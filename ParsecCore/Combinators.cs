using ParsecCore.Help;
using ParsecCore.ParsersHelp;
using System;
using System.Collections.Generic;

namespace ParsecCore
{
    public static class Combinators
    {
        /// <summary>
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails <strong>and does not consume any input</strong> then the second parser is applied and so on.
        /// If any parser fails while consuming input, then the parser's error is returned.
        /// If all parsers fail then combines the errors of all the parser,
        /// see: <see cref="ParseError.Combine(ParseError)"/>.
        /// <para/>
        /// Because the parser fails if any parser fails while consuming input the lookahead is 1.
        /// If there is need for parsing to continue in the case input is consumed, then consider modifying
        /// the parsers with the <see cref="ParserExt.Try{T, TInputToken}(Parser{T, TInputToken})"/> method.
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns>
        /// Parser which sequentally tries to apply the given parsers until one succeeds or all fails
        /// </returns>
        /// <exception cref="ArgumentNullException"> If parser array is null </exception>
        public static Parser<T, TInputToken> Choice<T, TInputToken>(
            params Parser<T, TInputToken>[] parsers
        )
        {
            if (parsers is null) throw new ArgumentNullException(nameof(parsers));

            return ChoiceParser.Parser(parsers);
        }

        /// <summary>
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails <strong>and does not consume any input</strong> then the second parser is applied and so on.
        /// If any parser fails while consuming input, then the parser's error is returned.
        /// If all parsers fail then returns the ParseError of tha last parser.
        /// 
        /// Because the parser fails if any parser fails while consuming input the lookahead is 1.
        /// If there is need for parsing to continue in the case input is consumed, then consider modifying
        /// the parsers with the <see cref="ParserExt.Try{T, TInputToken}(Parser{T, TInputToken})"/> method.
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns>
        /// Parser which sequentially tries to apply the given parsers until one succeeds or all fails
        /// </returns>
        /// <exception cref="ArgumentNullException"> If parser array is null </exception>
        public static Parser<T, TInputToken> Choice<T, TInputToken>(
            IEnumerable<Parser<T, TInputToken>> parsers
        )
        {
            if (parsers is null) throw new ArgumentNullException(nameof(parsers));

            return ChoiceParser.Parser(parsers);
        }

        /// <summary>
        /// Returns a parser which tries to parse all of the given parsers in a sequence.
        /// If it succeeds it returns an IReadOnlyList of the parsed results.
        /// If it fails it returns the first encountered parse error
        /// </summary>
        /// <typeparam name="T"> The type of parsers </typeparam>
        /// <param name="parsers"> Parsers to sequentially apply </param>
        /// <returns> Parser which sequentially aplies all of the given parsers </returns>
        /// <exception cref="ArgumentNullException"> If parser array is null </exception>
        public static Parser<IReadOnlyList<T>, TInputToken> All<T, TInputToken>(
            params Parser<T, TInputToken>[] parsers
        )
        {
            if (parsers is null) throw new ArgumentNullException(nameof(parsers));

            return AllParser.Parser(parsers);
        }

        /// <summary>
        /// Returns a parser which tries to parse all of the given parsers in a sequence.
        /// If it succeeds it returns an IReadOnlyList of the parsed results.
        /// If it fails it returns the first encountered parse error
        /// </summary>
        /// <typeparam name="T"> The type of parsers </typeparam>
        /// <param name="parsers"> Parsers to sequentially apply </param>
        /// <returns> Parser which sequentially aplies all of the given parsers </returns>
        /// <exception cref="ArgumentNullException"> If parser array is null </exception>
        public static Parser<IReadOnlyList<T>, TInputToken> All<T, TInputToken>(
            IEnumerable<Parser<T, TInputToken>> parsers
        )
        {
            if (parsers is null) throw new ArgumentNullException(nameof(parsers));
            
            return AllParser.Parser(parsers);
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
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<TBetween, TInputToken> Between<TLeft, TBetween, TRight, TInputToken>(
            Parser<TLeft, TInputToken> leftParser,
            Parser<TBetween, TInputToken> betweenParser,
            Parser<TRight, TInputToken> rightParser
        )
        {
            if (leftParser is null) throw new ArgumentNullException(nameof(leftParser));
            if (betweenParser is null) throw new ArgumentNullException(nameof(betweenParser));
            if (rightParser is null) throw new ArgumentNullException(nameof(rightParser));

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
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<TBetween, TInputToken> Between<TOutside, TBetween, TInputToken>(
            Parser<TOutside, TInputToken> outsideParser,
            Parser<TBetween, TInputToken> betweenParser
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
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<IReadOnlyList<TValue>, TInputToken> SepBy<TValue, TSeparator, TInputToken>(
            Parser<TValue, TInputToken> valueParser,
            Parser<TSeparator, TInputToken> separatorParser
        ) =>
            Choice(
                SepBy1(valueParser, separatorParser),
                Parsers.Return<IReadOnlyList<TValue>, TInputToken>(Array.Empty<TValue>())
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

            var sepValueParser = from sep in separatorParser
                                 from val in valueParser
                                 select val;

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

            return (from val in valueParser
                    from sep in separatorParser
                    select val).Many();
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

            return (from val in valueParser
                    from sep in separatorParser
                    select val).Many1();
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

        /// <summary>
        /// Parses zero or more occurences of the given values seperated by operators
        /// Returns a value obtained by <em>left-associative</em> application of the functions returned by op.
        /// If there are zero occurences, then the <c>defaultValue</c> is returned
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="value"> The parser for values </param>
        /// <param name="op"> Parser for the binary operators </param>
        /// <param name="defaultValue"> The default value to return in case no value is parsed </param>
        /// <returns> 
        /// Parser which returns a value obtained by the left-associative application of the functions 
        /// given by <c>op</c> on values returned by <c>value</c>
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<T, TInputToken> ChainL<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op,
            T defaultValue
        ) =>
            Choice(ChainL1(value, op), Parsers.Return<T, TInputToken>(defaultValue));

        /// <summary>
        /// Parses one or more occurences of the given values seperated by operators
        /// Returns a value obtained by <em>left-associative</em> application of the functions returned by op.
        /// Especially useful for parsing left-recursive grammars, which are often used in numerical expressions
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="value"> The parser for values </param>
        /// <param name="op"> Parser for the binary operators </param>
        /// <param name="defaultValue"> The default value to return in case no value is parsed </param>
        /// <returns> 
        /// Parser which returns a value obtained by the left-associative application of the functions 
        /// given by <c>op</c> on values returned by <c>value</c>
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<T, TInputToken> ChainL1<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (op is null) throw new ArgumentNullException(nameof(op));

            return Chainl1Parser.Parser(value, op);
        }

        /// <summary>
        /// Parses zero or more occurences of the given values seperated by operators
        /// Returns a value obtained by <em>right-associative</em> application of the functions returned by op.
        /// If there are zero occurences, then the <c>defaultValue</c> is returned
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="value"> The parser for values </param>
        /// <param name="op"> Parser for the binary operators </param>
        /// <param name="defaultValue"> The default value to return in case no value is parsed </param>
        /// <returns> 
        /// Parser which returns a value obtained by the right-associative application of the functions 
        /// given by <c>op</c> on values returned by <c>value</c>
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<T, TInputToken> ChainR<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op,
            T defaultValue
        ) =>
            Choice(ChainR1(value, op), Parsers.Return<T, TInputToken>(defaultValue));

        /// <summary>
        /// Parses one or more occurences of the given values seperated by operators
        /// Returns a value obtained by <em>right-associative</em> application of the functions returned by op.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <param name="value"> The parser for values </param>
        /// <param name="op"> Parser for the binary operators </param>
        /// <param name="defaultValue"> The default value to return in case no value is parsed </param>
        /// <returns> 
        /// Parser which returns a value obtained by the right-associative application of the functions 
        /// given by <c>op</c> on values returned by <c>value</c>
        /// </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<T, TInputToken> ChainR1<T, TInputToken>(
            Parser<T, TInputToken> value,
            Parser<Func<T, T, T>, TInputToken> op
        )
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (op is null) throw new ArgumentNullException(nameof(op));

            return Chainr1Parser.Parser(value, op);
        }

        /// <summary>
        /// This parser fails if <c>parser</c> succeeds. It does not consume any input.
        /// </summary>
        /// <typeparam name="T"> The return type of parser </typeparam>
        /// <param name="parser"> Parser which should not succeed </param>
        /// <param name="msgIfParsed"> The error message to use if <c>parser</c> succeeds </param>
        /// <returns> Parser which fails if <c>parser</c> succeeds </returns>
        /// <exception cref="ArgumentNullException"> If any arguments are null </exception>
        public static Parser<None, TInputToken> NotFollowedBy<T, TInputToken>(
            Parser<T, TInputToken> parser,
            string msgIfParsed
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (msgIfParsed is null) throw new ArgumentNullException(nameof(msgIfParsed));

            var failParser = from _ in parser.Try()
                             from fail in Parsers.Fail<None, TInputToken>(msgIfParsed)
                             select fail;
            return Choice(failParser, Parsers.Return<None, TInputToken>(new None())).Try();
        }

        /// <summary>
        /// Applies <c>parser</c> zero or more times until parser <c>till</c> succeeds.
        /// Returns the list of values returned by <c>parser</c>.
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
