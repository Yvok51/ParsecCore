﻿using ParsecCore.EitherNS;
using System;
using System.Collections.Generic;

namespace ParsecCore.ParsersHelp
{
    /// <summary>
    /// Parser which succeeds if the character passes a predicate.
    /// It consumes the read character only if the predicate is successfull
    /// </summary>
    class SatisfyParser
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
                var readPosition = input.Position;
                if (input.EndOfInput)
                {
                    return Either.Error<ParseError, char>(new ParseError("Unexpected end of file encountered", input.Position));
                }

                char read = input.Peek();

                if (!predicate(read))
                {
                    string readChar = escapedChars.ContainsKey(read) ? escapedChars[read] : read.ToString();
                    return Either.Error<ParseError, char>(new ParseError($"Character '{readChar}' does not conform, {predicateDescription} exprected", readPosition));
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
                var readPosition = input.Position;
                if (input.EndOfInput)
                {
                    return Either.Error<ParseError, TInputToken>(new ParseError("Unexpected end of file encountered", input.Position));
                }

                TInputToken read = input.Peek();

                if (!predicate(read))
                {
                    return Either.Error<ParseError, TInputToken>(new ParseError($"Token '{read}' does not conform, {predicateDescription} exprected", readPosition));
                }

                input.Read();

                return Either.Result<ParseError, TInputToken>(read);
            };
        }
    }
}
