using ParsecCore.Help;
using System;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which succeeds if the character passes a predicate.
    /// It consumes the read character only if the predicate is successfull
    /// </summary>
    internal class SatisfyParser
    {
        private static Dictionary<char, string> escapedChars = new Dictionary<char, string>()
        {
            { '\n', "\\n" },
            { '\b', "\\b" },
            { '\f', "\\f" },
            { '\r', "\\r" },
            { '\t', "\\t" },
            { '\v', "\\v" },
        };

        private static IResult<T, T> Error<T>(
            IParserInput<T> input,
            ErrorItem unexpected,
            IEnumerable<ErrorItem> description
        )
        {
            return Result.Failure<T, T>(
                new StandardError(
                    input.Position,
                    unexpected: Maybe.FromValue(unexpected),
                    expected: description
                ),
                input
            );
        }

        public static Parser<char, char> Parser(Predicate<char> predicate, string predicateDescription)
        {
            var description = new StringToken(predicateDescription).ToEnumerable();
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Error(input, EndOfFile.Instance, description);
                }

                char read = input.Current();

                if (!predicate(read))
                {
                    ErrorItem readChar = escapedChars.ContainsKey(read) ? new StringToken(escapedChars[read]) : new CharToken(read);
                    return Error(input, readChar, description);
                }

                return Result.Success(read, input.Advance());
            };
        }

        public static Parser<char, char> Parser(char expected, string predicateDescription)
        {
            var description = new StringToken(predicateDescription).ToEnumerable();
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Error(input, EndOfFile.Instance, description);
                }

                char read = input.Current();

                if (read != expected)
                {
                    ErrorItem readChar = escapedChars.ContainsKey(read) ? new StringToken(escapedChars[read]) : new CharToken(read);
                    return Error(input, readChar, description);
                }

                return Result.Success(read, input.Advance());
            };
        }

        public static Parser<TInputToken, TInputToken> Parser<TInputToken>(
            Predicate<TInputToken> predicate,
            string predicateDescription
        )
        {
            var description = new StringToken(predicateDescription).ToEnumerable();
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Error(input, EndOfFile.Instance, description);
                }

                TInputToken read = input.Current();

                if (!predicate(read))
                {
                    return Error(input, new SingleToken<TInputToken>(read), description);
                }

                return Result.Success(read, input.Advance());
            };
        }

        public static Parser<TInputToken, TInputToken> Parser<TInputToken>(
            TInputToken expected,
            string predicateDescription
        )
        {
            var description = new StringToken(predicateDescription).ToEnumerable();
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Error(input, EndOfFile.Instance, description);
                }

                TInputToken read = input.Current();

                if (!EqualityComparer<TInputToken>.Default.Equals(read, expected))
                {
                    return Error(input, new SingleToken<TInputToken>(read), description);
                }

                return Result.Success(read, input.Advance());
            };
        }
    }
}
