﻿using ParsecCore.EitherNS;
using ParsecCore.Help;
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
                    return Either.Error<ParseError, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(EndOfFile.Instance),
                            expected: new StringToken(predicateDescription)
                        )
                    );
                }

                char read = input.Peek();

                if (!predicate(read))
                {
                    string readChar = escapedChars.ContainsKey(read) ? escapedChars[read] : read.ToString();
                    return Either.Error<ParseError, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(new StringToken(readChar)),
                            expected: new StringToken(predicateDescription)
                        )
                    );
                }

                input.Read();

                return Either.Result<ParseError, char>(read);
            };
        }

        public static Parser<char, char> Parser(char expected, string predicateDescription)
        {
            return (input) =>
            {
                if (input.EndOfInput)
                {
                    return Either.Error<ParseError, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(EndOfFile.Instance),
                            expected: new StringToken(predicateDescription)
                        )
                    );
                }

                char read = input.Peek();

                if (read != expected)
                {
                    string readChar = escapedChars.ContainsKey(read) ? escapedChars[read] : read.ToString();
                    return Either.Error<ParseError, char>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(new StringToken(readChar)),
                            expected: new StringToken(predicateDescription)
                        )
                    );
                }

                input.Read();

                return Either.Result<ParseError, char>(read);
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
                    return Either.Error<ParseError, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(EndOfFile.Instance),
                            expected: new StringToken(predicateDescription)
                        )
                    );
                }

                TInputToken read = input.Peek();

                if (!predicate(read))
                {
                    return Either.Error<ParseError, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(new Token<TInputToken>(new[] { read })),
                            expected: new StringToken(predicateDescription)
                        )
                    );
                }

                input.Read();

                return Either.Result<ParseError, TInputToken>(read);
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
                    return Either.Error<ParseError, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(EndOfFile.Instance),
                            expected: new StringToken(predicateDescription)
                        )
                    );
                }

                TInputToken read = input.Peek();

                if (!EqualityComparer<TInputToken>.Default.Equals(read, expected))
                {
                    return Either.Error<ParseError, TInputToken>(
                        new StandardError(
                            input.Position,
                            unexpected: Maybe.FromValue(new Token<TInputToken>(new[] { read })),
                            expected: new StringToken(predicateDescription)
                        )
                    );
                }

                input.Read();

                return Either.Result<ParseError, TInputToken>(read);
            };
        }
    }
}