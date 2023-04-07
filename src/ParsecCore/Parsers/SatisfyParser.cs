using ParsecCore.EitherNS;
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
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Failure<char, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(EndOfFile.Instance),
                            expected: new StringToken(predicateDescription)
                        ),
                        input
                    );
                }

                char read = input.Current();

                if (!predicate(read))
                {
                    string readChar = escapedChars.ContainsKey(read) ? escapedChars[read] : read.ToString();
                    return Result.Failure<char, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(new StringToken(readChar)),
                            expected: new StringToken(predicateDescription)
                        ),
                        input
                    );
                }

                return Result.Success(read, input.Advance());
            };
        }

        public static Parser<char, char> Parser(char expected, string predicateDescription)
        {
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Failure<char, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(EndOfFile.Instance),
                            expected: new StringToken(predicateDescription)
                        ),
                        input
                    );
                }

                char read = input.Current();

                if (read != expected)
                {
                    string readChar = escapedChars.ContainsKey(read) ? escapedChars[read] : read.ToString();
                    return Result.Failure<char, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(new StringToken(readChar)),
                            expected: new StringToken(predicateDescription)
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
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Failure<TInputToken, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(EndOfFile.Instance),
                            expected: new StringToken(predicateDescription)
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
                            unexpected: Maybe.FromValue<ErrorItem>(new Token<TInputToken>(new[] { read })),
                            expected: new StringToken(predicateDescription)
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
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Result.Failure<TInputToken, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue<ErrorItem>(EndOfFile.Instance),
                            expected: new StringToken(predicateDescription)
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
                            unexpected: Maybe.FromValue<ErrorItem>(new Token<TInputToken>(new[] { read })),
                            expected: new StringToken(predicateDescription)
                        ),
                        input
                    );
                }

                return Result.Success(read, input.Advance());
            };
        }
    }
}
