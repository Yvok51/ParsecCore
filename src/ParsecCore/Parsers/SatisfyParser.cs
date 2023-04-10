using ParsecCore.MaybeNS;
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

        public static Parser<char, char> Parser(Predicate<char> predicate, string predicateDescription)
        {
            var description = new StringToken(predicateDescription);
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Failure<char, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(EndOfFile.Instance),
                            expected: description
                        ),
                        input
                    );
                }

                char read = input.Current();

                if (!predicate(read))
                {
                    ErrorItem readChar = escapedChars.ContainsKey(read) ? new StringToken(escapedChars[read]) : new CharToken(read);
                    return Result.Failure<char, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(readChar),
                            expected: description
                        ),
                        input
                    );
                }

                return Result.Success(read, input.Advance());
            };
        }

        public static Parser<char, char> Parser(char expected, string predicateDescription)
        {
            var description = new StringToken(predicateDescription);
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Failure<char, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(EndOfFile.Instance),
                            expected: description
                        ),
                        input
                    );
                }

                char read = input.Current();

                if (read != expected)
                {
                    ErrorItem readChar = escapedChars.ContainsKey(read) ? new StringToken(escapedChars[read]) : new CharToken(read);
                    return Result.Failure<char, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(readChar),
                            expected: description
                        ),
                        input
                    );
                }

                return Result.Success(read, input.Advance());
            };
        }

        public static Parser<TInputToken, TInputToken> Parser<TInputToken>(
            Predicate<TInputToken> predicate,
            string predicateDescription
        )
        {
            var description = new StringToken(predicateDescription);
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Failure<TInputToken, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(EndOfFile.Instance),
                            expected: description
                        ),
                        input
                    );
                }

                TInputToken read = input.Current();

                if (!predicate(read))
                {
                    return Result.Failure<TInputToken, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(new SingleToken<TInputToken>(read)),
                            expected: description
                        ),
                        input
                    );
                }

                return Result.Success(read, input.Advance());
            };
        }

        public static Parser<TInputToken, TInputToken> Parser<TInputToken>(
            TInputToken expected,
            string predicateDescription
        )
        {
            var description = new StringToken(predicateDescription);
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Failure<TInputToken, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(EndOfFile.Instance),
                            expected: description
                        ),
                        input
                    );
                }

                TInputToken read = input.Current();

                if (!EqualityComparer<TInputToken>.Default.Equals(read, expected))
                {
                    return Result.Failure<TInputToken, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(new SingleToken<TInputToken>(read)),
                            expected: description
                        ),
                        input
                    );
                }

                return Result.Success(read, input.Advance());
            };
        }
    }
}
