﻿using ParsecCore.Help;
using ParsecCore.Either;
using ParsecCore.Input;

namespace ParsecCore
{
    /// <summary>
    /// Parser which parses the end of the file.
    /// Fails if any character is present
    /// </summary>
    public class EOFParser : IParser<None>
    {
        public IEither<ParseError, None> Parse(IParserInput input)
        {
            if (input.EndOfInput)
            {
                return new Result<ParseError, None>(new None());
            }

            return new Error<ParseError, None>(new ParseError("Unexpected char encountered, expected EOF", input.Position));
        }
    }
}
