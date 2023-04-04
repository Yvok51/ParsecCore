using ParsecCore.EitherNS;
using ParsecCore.Input;
using System;
using System.Linq;

namespace ParsecCore
{
    public static partial class Parsers
    {

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

        /// <summary>
        /// Changes the error message returned by the parser if it fails.
        /// The parser modified by this method behaves the same except that when it fails,
        /// the expected message is set to only contain the new provided error message.
        /// </summary>
        /// <typeparam name="T"> The type of parser </typeparam>
        /// <typeparam name="TInputToken"> The type of the token returned by the parser's input </typeparam>
        /// <param name="parser"> The parser whose error message to change </param>
        /// <param name="newExpectedMessage"> The new expected error message </param>
        /// <returns> Parser which upon failure returns ParseError with modified error message </returns>
        /// <exception cref="ArgumentNullException"> If any of the arguments are null </exception>
        public static Parser<T, TInputToken> FailWith<T, TInputToken>(
            this Parser<T, TInputToken> parser,
            string newExpectedMessage
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (newExpectedMessage is null) throw new ArgumentNullException(nameof(newExpectedMessage));

            return (input) =>
            {
                var result = parser(input);
                if (result.IsResult)
                {
                    return result;
                }

                return Either.Error<ParseError, T>(
                    result.Error.Accept(FailWithVisitor.Instance, newExpectedMessage)
                );
            };
        }

        /// <summary>
        /// Visitor for implementing <see cref="FailWith{T, TInputToken}(Parser{T, TInputToken}, string)"/>
        /// since it behaves differently for the two types of errors.
        /// </summary>
        private class FailWithVisitor : IParseErrorVisitor<ParseError, string>
        {
            private FailWithVisitor() { }

            public static readonly FailWithVisitor Instance = new();

            public ParseError Visit(StandardError error, string msg)
            {
                return new StandardError(error.Position, error.Unexpected, new StringToken(msg));
            }

            public ParseError Visit(CustomError error, string msg)
            {
                return new CustomError(error.Position, error.Customs.Append(new FailWithError(msg)));
            }
        }


        /// <summary>
        /// Assert that the parser result fulfills a predicate.
        /// 
        /// Tries to parse using <paramref name="parser"/>.
        /// If parse succeeds then tests the result with predicate.
        /// If predicate fails then an error is generated using <paramref name="generateError"/>
        /// </summary>
        /// <typeparam name="T"> The return type of <paramref name="parser"/> </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to parse with </param>
        /// <param name="predicate"> Test for the result of the parse </param>
        /// <param name="errorMessage"> What message to report in case result does not pass test </param>
        /// <returns> Parser which tests whether the result of the parse passes a test </returns>
        /// <exception cref="ArgumentNullException">  If any of the arguments are null </exception>
        public static Parser<T, TInputToken> Assert<T, TInputToken>(
            this Parser<T, TInputToken> parser,
            Func<T, bool> predicate,
            string errorMessage
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (errorMessage is null) throw new ArgumentNullException(nameof(errorMessage));

            return parser.Assert(predicate, (_, pos) => new CustomError(pos, new FailWithError(errorMessage)));
        }

        /// <summary>
        /// Assert that the parser result fulfills a predicate.
        /// 
        /// Tries to parse using <paramref name="parser"/>.
        /// If parse succeeds then tests the result with predicate.
        /// If predicate fails then an error is generated using <paramref name="generateError"/>
        /// </summary>
        /// <typeparam name="T"> The return type of <paramref name="parser"/> </typeparam>
        /// <typeparam name="TInputToken"> Type of the input token </typeparam>
        /// <param name="parser"> The parser to parse with </param>
        /// <param name="predicate"> Test for the result of the parse </param>
        /// <param name="generateError"> How to generate error if predicate fails </param>
        /// <returns> Parser which tests whether the result of the parse passes a test </returns>
        /// <exception cref="ArgumentNullException">  If any of the arguments are null </exception>
        public static Parser<T, TInputToken> Assert<T, TInputToken>(
            this Parser<T, TInputToken> parser,
            Func<T, bool> predicate,
            Func<T, Position, ParseError> generateError
        )
        {
            if (parser is null) throw new ArgumentNullException(nameof(parser));
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));
            if (generateError is null) throw new ArgumentNullException(nameof(generateError));

            return (input) =>
            {
                var position = input.Position;
                var result = parser(input);
                return result.Match(
                    right: value => predicate(value)
                        ? Either.Result<ParseError, T>(value)
                        : Either.Error<ParseError, T>(generateError(value, position)),
                    left: () => result
                );
            };
        }
    }
}
