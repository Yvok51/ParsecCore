using ParsecCore.EitherNS;
using ParsecCore.ParsersHelp;
using System;
using System.Collections.Generic;

namespace ParsecCore
{
    public static partial class Parsers
    {
        /// <summary>
        /// Returns a parser which tries to apply the first parser and if it succeeds returns the result.
        /// If it fails <strong>and does not consume any input</strong> then the second parser is tried.
        /// If the first parser fails while consuming input, then the first parser's error is returned.
        /// If both parsers fail then combines their errors,
        /// see <see cref="ParseError.Accept{T, A}(IParseErrorVisitor{T, A}, A)"/>.
        /// <para/>
        /// Because the parser fails if the first parser fails while consuming input the lookahead is 1.
        /// If there is need for parsing to continue in the case input is consumed, then consider modifying
        /// the first parser with the <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> method.
        /// </summary>
        /// <typeparam name="T"> The return type of the parser </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="firstParser"> First parser to try </param>
        /// <param name="secondParser"> Second parser to try </param>
        /// <returns> Parser whose result is the result of the first parser to succeed </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, TInputToken> Or<T, TInputToken>(
            this Parser<T, TInputToken> firstParser,
            Parser<T, TInputToken> secondParser
        )
        {
            if (firstParser is null) throw new ArgumentNullException(nameof(firstParser));
            if (secondParser is null) throw new ArgumentNullException(nameof(secondParser));

            return (input) =>
            {
                var initialPosition = input.Position;
                var firstResult = firstParser(input);
                if (firstResult.IsResult || (firstResult.IsError && initialPosition != input.Position))
                {
                    return firstResult;
                }

                var secondResult = secondParser(input);
                if (secondResult.IsError)
                {
                    return firstResult.CombineErrors(secondResult);
                }

                return secondResult;
            };
        }

        /// <summary>
        /// Returns a parser which tries to first apply the first parser and if it succeeds returns the result.
        /// If it fails <strong>and does not consume any input</strong> then the second parser is applied and so on.
        /// If any parser fails while consuming input or if all parsers fail,
        /// then combines the errors of all the parsers,
        /// see <see cref="ParseError.Accept{T, A}(IParseErrorVisitor{T, A}, A)"/>.
        /// <para/>
        /// <para>
        /// Because the parser fails if any parser fails while consuming input the lookahead is 1.
        /// If there is need for parsing to continue in the case input is consumed, then consider modifying
        /// the parsers with the <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> method.
        /// </para>
        /// <para>
        /// If no parsers are provided then returns an error "No parsers provided".
        /// </para>
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns>
        /// Parser which sequentally tries to apply the given parsers until one succeeds or all fail
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
        /// If any parser fails while consuming input or if all parsers fail,
        /// then combines the errors of all the parsers,
        /// see <see cref="ParseError.Accept{T, A}(IParseErrorVisitor{T, A}, A)"/>.
        /// <para/>
        /// <para>
        /// Because the parser fails if any parser fails while consuming input the lookahead is 1.
        /// If there is need for parsing to continue in the case input is consumed, then consider modifying
        /// the parsers with the <see cref="Try{T, TInputToken}(Parser{T, TInputToken})"/> method.
        /// </para>
        /// <para>
        /// If no parsers are provided then returns an error "No parsers provided".
        /// </para>
        /// </summary>
        /// <typeparam name="T"> The type of the parsers </typeparam>
        /// <param name="parsers"> Parsers to apply </param>
        /// <returns>
        /// Parser which sequentially tries to apply the given parsers until one succeeds or all fail
        /// </returns>
        /// <exception cref="ArgumentNullException"> If parser array is null </exception>
        public static Parser<T, TInputToken> Choice<T, TInputToken>(
            IEnumerable<Parser<T, TInputToken>> parsers
        )
        {
            if (parsers is null) throw new ArgumentNullException(nameof(parsers));

            return ChoiceParser.Parser(parsers);
        }
    }
}
