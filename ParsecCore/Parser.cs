using ParsecCore.EitherNS;
using ParsecCore.Input;

namespace ParsecCore
{
    public delegate IEither<ParseError, T> Parser<out T>(IParserInput<char> input);
}
