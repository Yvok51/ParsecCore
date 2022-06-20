using System;

namespace ParsecCore
{
    interface IParser<T>
    {
        IEither<ParseError, T> Parse(IParserInput input);
    }
}
