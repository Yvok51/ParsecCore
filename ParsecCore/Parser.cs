using ParsecCore.Input;
using ParsecCore.EitherNS;

namespace ParsecCore
{
    public delegate IEither<ParseError, T> Parser<out T>(IParserInput input);
}
